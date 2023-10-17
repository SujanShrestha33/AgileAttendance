import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendenceLogComponent } from './attendence-log.component';

describe('AttendenceLogComponent', () => {
  let component: AttendenceLogComponent;
  let fixture: ComponentFixture<AttendenceLogComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AttendenceLogComponent]
    });
    fixture = TestBed.createComponent(AttendenceLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
