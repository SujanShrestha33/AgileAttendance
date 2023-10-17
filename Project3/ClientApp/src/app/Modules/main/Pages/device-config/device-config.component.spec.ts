import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceConfigComponent } from './device-config.component';

describe('DeviceConfigComponent', () => {
  let component: DeviceConfigComponent;
  let fixture: ComponentFixture<DeviceConfigComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DeviceConfigComponent]
    });
    fixture = TestBed.createComponent(DeviceConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
