import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(private http: HttpClient) { }

  uri:String = "https://localhost:6160/categories";

  getCategories(){
    return this.http.get(`${this.uri}`);
  }
}
