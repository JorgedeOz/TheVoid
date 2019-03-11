import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { OrderTicketModel as OrderTicketModel } from 'src/app/models/OrderTicketModel';
import { ApiService } from 'src/app/services/api.service';
import { HttpErrorResponse } from '@angular/common/http';

declare var bootbox:any;
declare var $: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit  {

  public Orders: Array<OrderTicketModel> = [];
  @ViewChild('fileInput') fileInput: ElementRef;
  public showAlert: boolean = true;
  public alertMessage: string = "";
  public alertType: string = "";
  public CurrentOrder: OrderTicketModel;
  public buttonDisabled: boolean = false;
  public saveButtonText: string = "Save";

  constructor(private apiService: ApiService) {
    this.CurrentOrder = new OrderTicketModel();
  }

  ngOnInit() {
    this.showLoading();
    this.apiService.GetOrderTickets().subscribe(
      (data) => this.Orders = data,
      (error:HttpErrorResponse) =>{        
        this.showError("An error ocurred while getting the orders",error);        
      },
      () => this.showAlert = false
    );
  }

  private showLoading(info?:string){
    this.showAlert = true;
    this.alertType = "alert-primary";
    this.alertMessage = `Loading please wait...`;
    if(info != undefined) this.alertMessage = info;
  }

  private showError(errorDesc: string, error:HttpErrorResponse){
    this.showAlert = true;
    this.alertType = "alert-danger";
    this.alertMessage = `${errorDesc}: "${error.statusText}"`;
    console.error(error);    
  }

  private showSuccess(description: string){
    this.showAlert = true;
    this.alertType = "alert-success";
    this.alertMessage = description;
  }

  showBrowser(){
    this.fileInput.nativeElement.click();
  }

  onFileChange(event){
    if(event.target.files && event.target.files.length > 0) {
      let file = event.target.files[0];
      if(file.type != "application/vnd.ms-excel"){
        bootbox.alert(`The selected file ${file.name} is not valid!`);
        return;
      }
      let data = new FormData();      
      data.append('InputFile', file,file.name);
      this.showLoading("Uploading and saving data please wait...");
      this.apiService.UploadFile(data).subscribe(
        (data) => {
          this.Orders = data;
          this.showSuccess("CSV data imported successfuly");
        },
        (error) => this.showError("An error ocurred while uploading the file",error),
        () => {
          this.fileInput.nativeElement.value = "";
          this.showAlert = false;
        }
      );      
    }
  }

  showModal(){
    this.CurrentOrder = new OrderTicketModel();
    $("#myModal").modal('show');
  }

  show(order: OrderTicketModel){
    this.CurrentOrder =  JSON.parse(JSON.stringify(order));
    this.CurrentOrder.EventDate = this.CurrentOrder.EventDate.toString().substring(0,10);
    //$("#date").val();
    $("#myModal").modal('show');
  }

  saveOrder(){
    this.saveButtonText = "Saving...";
    this.buttonDisabled = true;
    this.apiService.SaveOrder(this.CurrentOrder).subscribe(
      (data) => {
        this.Orders = data;
        this.showSuccess("Order saved successfuly");
      },
      (error) => {
        this.showError("An error ocurred while saving the order",error);        
        this.saveButtonText = "Save";
        this.buttonDisabled = false;
      },
      () => {
        this.saveButtonText = "Save";
        this.buttonDisabled = false;
        $("#myModal").modal('hide');
      }
    );
  }

  delete(order: OrderTicketModel){
    bootbox.confirm("Are you sure to remove the order?",(r)=>{
      if(r){
        this.showLoading("Deleting the order please wait...");
        this.apiService.DeleteOrder(order.OrderId,order.TicketId).subscribe(
          (data) => {
            this.Orders = data;
            this.showSuccess("Order deleted successfuly");
          },
          (error) => this.showError("An error ocurred while deleting the order",error)          
        );
      }
    });
  }

}
