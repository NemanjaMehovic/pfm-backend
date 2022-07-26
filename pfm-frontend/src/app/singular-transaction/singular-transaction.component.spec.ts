import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SingularTransactionComponent } from './singular-transaction.component';

describe('SingularTransactionComponent', () => {
  let component: SingularTransactionComponent;
  let fixture: ComponentFixture<SingularTransactionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SingularTransactionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SingularTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
