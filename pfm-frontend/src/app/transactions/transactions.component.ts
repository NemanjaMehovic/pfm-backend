import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Category } from '../models/categorie';
import { Transaction } from '../models/transaction';
import { TransactionsList } from '../models/transactionsList';
import { CategoryService } from '../services/category.service';
import { TransactionService } from '../services/transaction.service';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']
})
export class TransactionsComponent implements OnInit {

  constructor(private router: Router, private service: TransactionService, private serviceC: CategoryService) { }

  ngOnInit(): void {
    this.loadTransactions()
    this.serviceC.getCategories().subscribe((tmp:any)=>{
      this.categories = tmp.items;
      for(let i = 0; i < this.categories.length; i++)
        if(this.categories[i].code == "Z")
        {
          this.categories.splice(i,1);
          break;
        }

    });
  }

  tip: Array<String> = ["dep", "wdw", "pmt", "fee", "inc", "rev", "adj", "lnd", "lnr", "fcx", "aop", "acl", "spl", "sal"];
  sortBy: Array<String> = ["id", "beneficiary-name", "date", "direction", "amount", "description", "currency", "mcc", "kind"];
  sortOrder: Array<String> = ["asc", "desc"];
  categories: Array<Category> = [];

  currPageNum: number = 1;
  maxPages: number = 0;
  transactionList: TransactionsList = new TransactionsList();

  by: String | null = null;
  order: String | null = null;
  kind: String | null = null;
  startDate: Date | null = null;
  endDate: Date | null = null;

  search(form: NgForm) {
    this.by = form.value.sortBy != "" ? form.value.sortBy : null;
    this.order = form.value.sortOrder != "" ? form.value.sortOrder : null;
    this.kind = form.value.tip != "" ? form.value.tip : null;
    this.startDate = form.value.startDate != "" ? form.value.startDate : null;
    this.endDate = form.value.endDate != "" ? form.value.endDate : null;
    this.currPageNum = 1;
    this.loadTransactions();
  }


  loadTransactions() {
    this.service.getTransactions(this.kind, this.by, this.order, this.startDate, this.endDate, this.currPageNum).subscribe((tmp: any) => {
      this.transactionList = tmp;
      this.transactionList.page_size = tmp['page-size'];
      this.transactionList.sort_by = tmp['sort-by'];
      this.transactionList.sort_order = tmp['sort-order'];
      this.transactionList.total_count = tmp['total-count'];
      if (this.transactionList.items != undefined)
        for (let i = 0; i < this.transactionList.items?.length; i++)
          this.transactionList.items[i].beneficiary_name = tmp.items[i]['beneficiary-name'];
      this.currPageNum = this.transactionList.page != undefined ? this.transactionList.page : 1;
      this.maxPages = this.transactionList.total_count != undefined ? this.transactionList.total_count : 1;
    });
  }

  next() {
    if((this.currPageNum + 1) <= this.maxPages)
      this.currPageNum += 1;
    this.loadTransactions();
  }

  prev() {
    if((this.currPageNum - 1) >= 1)
      this.currPageNum -= 1;
    this.loadTransactions();
  }

}
