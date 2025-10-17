import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { TasksService } from './tasks.service';

describe('TasksService', () => {
  let svc: TasksService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    svc = TestBed.inject(TasksService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('list uses query params', () => {
    svc.list({ status: 'open', sort: 'createdAt', direction: 'desc' }).subscribe();
    const req = http.expectOne(r => r.url === `${environment.apiUrl}/tasks`);
    expect(req.request.params.get('status')).toBe('open');
    expect(req.request.params.get('sort')).toBe('createdAt');
    expect(req.request.params.get('direction')).toBe('desc');
    req.flush([]);
  });

  it('create posts title', () => {
    svc.create('Task X').subscribe();
    const req = http.expectOne(`${environment.apiUrl}/tasks`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ title: 'Task X' });
    req.flush({ id: 1, title: 'Task X', isDone: false, orderIndex: 1, createdAt: new Date().toISOString() });
  });
});

