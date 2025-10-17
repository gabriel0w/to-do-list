export interface TaskItem {
  id: number;
  title: string;
  isDone: boolean;
  orderIndex: number;
  createdAt: string;
}

export type TaskStatusFilter = 'all' | 'open' | 'done';
export type TaskSortField = 'orderIndex' | 'createdAt';
export type SortDirection = 'asc' | 'desc';

