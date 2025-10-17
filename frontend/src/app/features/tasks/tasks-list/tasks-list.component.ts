import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { TasksService } from '../../../core/services/tasks.service';
import { TaskItem } from '../../../core/models/task';

@Component({
  selector: 'app-tasks-list',
  standalone: true,
  imports: [CommonModule, MatListModule, MatButtonModule, MatIconModule, MatSnackBarModule, DragDropModule],
  templateUrl: './tasks-list.component.html',
  styleUrls: ['./tasks-list.component.scss']
})
export class TasksListComponent implements OnInit {
  private svc = inject(TasksService);
  private snack = inject(MatSnackBar);

  items: TaskItem[] = [];
  loading = false;

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading = true;
    this.svc.list().subscribe({
      next: (res) => { this.items = res; this.loading = false; },
      error: () => { this.snack.open('Failed to load tasks', 'Close', { duration: 2500 }); this.loading = false; }
    });
  }

  toggle(item: TaskItem) {
    this.svc.toggleComplete(item.id).subscribe({
      next: (updated) => { item.isDone = updated.isDone; },
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

