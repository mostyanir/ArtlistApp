/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PathListComponent } from './path-list.component';

describe('PathListComponent', () => {
  let component: PathListComponent;
  let fixture: ComponentFixture<PathListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PathListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PathListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
