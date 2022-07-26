import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Transaction } from '../models/transaction';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  constructor(private http: HttpClient) { }

  uri: String = 'https://localhost:6160/transactions';

  getTransactions(kind: String | null, by: String | null, order: String | null, startDate: Date | null, endDate: Date | null, page: Number) {
    let tmp: Array<string> = [];
    if (kind != null)
      tmp.push("transaction-kind=" + encodeURIComponent(kind.toString()));
    if (by != null)
      tmp.push("sort-by=" + encodeURIComponent(by.toString()));
    if (order != null)
      tmp.push("sort-order=" + encodeURIComponent(order.toString()));
    if (startDate != null)
      tmp.push("start-date=" + encodeURIComponent(startDate.toString()));
    if (endDate != null)
      tmp.push("end-date=" + encodeURIComponent(endDate.toString()));
    tmp.push("page=" + encodeURIComponent(page.toString()));
    tmp.push("page-size=10");
    let tmpS: string = tmp.join("&");
    return this.http.get(`${this.uri}?` + tmpS);
  }

  categorize(transaction: Transaction) {
    return this.http.post(`${this.uri}/` + transaction.id + "/categorize", { "catcode": transaction.catcode }, { responseType: 'text' });
  }
}
