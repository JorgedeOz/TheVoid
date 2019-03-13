import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OrderTicketModel } from '../models/OrderTicketModel';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {  

  private API_ENDPOINT: string = "http://localhost:5000/Main";

  constructor(private http:HttpClient) { }

  public GetOrderTickets(): Observable<Array<OrderTicketModel>>{
    return this.http.get<Array<OrderTicketModel>>(`${this.API_ENDPOINT}/OrderTickets`);
  }

  public SaveOrder(data: OrderTicketModel): Observable<Array<OrderTicketModel>>{ 
    let headers = new HttpHeaders({ 'Content-Type': 'application/json'});
    let body = JSON.stringify(data);
    return this.http.post<Array<OrderTicketModel>>(`${this.API_ENDPOINT}`,data,{headers:headers});
  }

  public UploadFile(data: FormData): Observable<Array<OrderTicketModel>>{ 
    return this.http.post<Array<OrderTicketModel>>(`${this.API_ENDPOINT}/UploadFile`,data);
  }

  public DeleteOrder(id: number,ticketId:number): Observable<Array<OrderTicketModel>>{ 
    return this.http.delete<Array<OrderTicketModel>>(`${this.API_ENDPOINT}/${id}/${ticketId}`);
  }
}
