import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { TaskItem, TaskSortField, TaskStatusFilter, SortDirection } from '../models/task';

@Injectable({ providedIn: 'root' })
export class TasksService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/tasks`;

  list(filter?: { status?: TaskStatusFilter; sort?: TaskSortField; direction?: SortDirection }) {
    let params = new HttpParams();
    if (filter?.status) params = params.set('status', filter.status);
    if (filter?.sort) params = params.set('sort', filter.sort);
    if (filter?.direction) params = params.set('direction', filter.direction);
    return this.http.get<TaskItem[]>(this.base, { params });
  }

  get(id: number) {
    return this.http.get<TaskItem>(`${this.base}/${id}`);
  }

  create(title: string) {
    return this.http.post<TaskItem>(this.base, { title });
  }

  toggleComplete(id: number) {
    return this.http.patch<TaskItem>(`${this.base}/${id}/complete`, {});
  }

  delete(id: number) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  reorder(items: { id: number; orderIndex: number }[]) {
    return this.http.put<void>(`${this.base}/reorder`, { items });
  }
}

