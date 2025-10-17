import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { TasksListComponent } from './tasks-list.component';
import { TasksService } from '../../../core/services/tasks.service';

describe('TasksListComponent', () => {
  let component: TasksListComponent;
  let fixture: ComponentFixture<TasksListComponent>;
  const svcSpy = jasmine.createSpyObj('TasksService', ['list', 'create', 'toggleComplete', 'delete', 'reorder']);

  beforeEach(async () => {
    svcSpy.list.and.returnValue(of([]));
    await TestBed.configureTestingModule({
      imports: [TasksListComponent],
      providers: [{ provide: TasksService, useValue: svcSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(TasksListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should load tasks on init', () => {
    expect(svcSpy.list).toHaveBeenCalled();
  });

  it('should call create on onCreate', () => {
    svcSpy.create.and.returnValue(of({ id: 1, title: 'X', isDone: false, orderIndex: 1, createdAt: new Date().toISOString() }));
    component.form.setValue({ title: 'X' });
    component.onCreate();
    expect(svcSpy.create).toHaveBeenCalledWith('X');
  });
});

