import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListPasswordsComponent } from './password-list.component';

describe('ListPasswordsComponent', () => {
  let component: ListPasswordsComponent;
  let fixture: ComponentFixture<ListPasswordsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListPasswordsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListPasswordsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
