import { Component, Input, OnInit } from '@angular/core';
import { Category } from '../models/categorie';
import { Transaction } from '../models/transaction';
import { TransactionService } from '../services/transaction.service';

@Component({
  selector: 'app-singular-transaction',
  templateUrl: './singular-transaction.component.html',
  styleUrls: ['./singular-transaction.component.css']
})
export class SingularTransactionComponent implements OnInit {

  constructor(private service: TransactionService) { }

  ngOnInit(): void {
  }

  @Input() transaction: Transaction = new Transaction();
  @Input() categories: Array<Category> = [];
  catcode: String = "";

  date() {
    if (this.transaction.date != undefined) {
      let tmp: string = this.transaction.date.toString();
      tmp = tmp.slice(0, tmp.indexOf('T'));
      let x: Array<string> = tmp.split("-");
      x = x.reverse();
      tmp = x.join(".");
      return tmp;
    }
    return "";
  }

  categorize() {
    if (this.catcode != "")
      this.transaction.catcode = this.catcode;

    this.service.categorize(this.transaction).subscribe((tmp: any) => {
      console.log(tmp);
    });
  }

  nameOfCategory(catcode: String) {
    if (catcode == "Z")
      return "Split";
    for (let i = 0; i < this.categories.length; i++)
      if (this.categories[i].code == catcode)
        return this.categories[i].name;
    return "Unknown category"
  }
}
