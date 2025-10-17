import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { TasksService } from '../../../core/services/tasks.service';
import { NotificationService } from '../../../core/services/notification.service';
import { TaskItem, TaskSortField, TaskStatusFilter, SortDirection } from '../../../core/models/task';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-tasks-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressBarModule,
    DragDropModule
  ],
  templateUrl: './tasks-list.component.html',
  styleUrls: ['./tasks-list.component.scss']
})
export class TasksListComponent implements OnInit {
  private svc = inject(TasksService);
  private snack = inject(MatSnackBar);
  private notifier = inject(NotificationService);
  private fb = inject(FormBuilder);

  items: TaskItem[] = [];
  loading = false;
  creating = false;

  // Filters
  status: TaskStatusFilter = 'all';
  sort: TaskSortField = 'orderIndex';
  direction: SortDirection = 'asc';

  // Form
  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]]
  });

  ngOnInit(): void {
    // Request permission for browser notifications (non-blocking)
    this.notifier.requestPermissionIfNeeded().finally(() => {});
    this.load();
  }

  load() {
    this.loading = true;
    this.svc.list({ status: this.status, sort: this.sort, direction: this.direction }).subscribe({
      next: (res) => { this.items = res; this.loading = false; },
      error: () => { this.snack.open('Failed to load tasks', 'Close', { duration: 2500 }); this.loading = false; }
    });
  }

  onFilterChange() {
    this.load();
  }

  onCreate() {
    if (this.form.invalid) return;
    const title = this.form.value.title!.trim();
    if (!title) return;
    this.creating = true;
    this.svc.create(title).subscribe({
      next: (created) => {
        this.items.push(created);
        // keep order by orderIndex
        this.items.sort((a, b) => a.orderIndex - b.orderIndex || a.createdAt.localeCompare(b.createdAt));
        this.form.reset();
        this.snack.open('Task created', 'Close', { duration: 1500 });
        this.creating = false;
      },
      error: () => { this.snack.open('Failed to create', 'Close', { duration: 2000 }); this.creating = false; }
    });
  }

  toggle(item: TaskItem) {
    this.svc.toggleComplete(item.id).subscribe({
      next: (updated) => {
        item.isDone = updated.isDone;
        if (item.isDone) {
          this.notifier.notify('Task completed', item.title);
        } else {
          this.snack.open('Task reopened', 'Close', { duration: 1500 });
        }
        // Keep view consistent with current filter
        if (this.status === 'open' && item.isDone) {
          this.items = this.items.filter(i => i.id !== item.id);
        }
        if (this.status === 'done' && !item.isDone) {
          this.items = this.items.filter(i => i.id !== item.id);
        }
      },
      error: () => this.snack.open('Failed to toggle', 'Close', { duration: 2000 })
    });
  }

  remove(item: TaskItem) {
    this.svc.delete(item.id).subscribe({
      next: () => { this.items = this.items.filter(i => i.id !== item.id); },
      error: () => this.snack.open('Failed to delete', 'Close', { duration: 2000 })
    });
  }

  drop(event: CdkDragDrop<TaskItem[]>) {
    if (event.previousIndex === event.currentIndex) return;
    moveItemInArray(this.items, event.previousIndex, event.currentIndex);
    const payload = this.items.map((t, idx) => ({ id: t.id, orderIndex: idx + 1 }));
    this.svc.reorder(payload).subscribe({
      next: () => this.snack.open('Order updated', 'Close', { duration: 1500 }),
      error: () => this.snack.open('Failed to reorder', 'Close', { duration: 2000 })
    });
  }
}
