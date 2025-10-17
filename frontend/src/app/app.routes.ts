import { Routes } from '@angular/router';
import { TASKS_ROUTES } from './features/tasks/tasks.routes';

export const routes: Routes = [
  { path: '', redirectTo: 'tasks', pathMatch: 'full' },
  { path: 'tasks', children: TASKS_ROUTES }
];
