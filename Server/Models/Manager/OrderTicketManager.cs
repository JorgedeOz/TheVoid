using System.Collections.Generic;
using TheVoid.Models.DataBase;
using TheVoid.Models.ResponseTypes;
using System.Linq;
using System.Transactions;
using static TheVoid.Models.DataBase.DBContext;
using Microsoft.EntityFrameworkCore;
using System;

namespace TheVoid.Models.Manager{
    public class OrderTicketManager{

        public IEnumerable<OrderTicketModel> GetAllOrderTickets(){
            IEnumerable<OrderTicketModel> response = null;
            using(var db = new DBContext()){
                response = 
                (
                    from o in db.Orders 
                    join t in db.Tickets on o.Order_Id equals t.Order_Id
                    select new OrderTicketModel()
                    {
                        OrderId = o.Order_Id,
                        FirstName = o.First_Name,
                        LastName = o.Last_Name,
                        TicketNumber = t.Ticket_Number,
                        EventDate = t.Event_Date,
                        TicketId = t.Ticket_Id
                    }
                ).ToList();                
            }
            return response;
        }

        public void ImportCSVData(List<string[]> data){
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using(var db = new DBContext())
                {                        
                    foreach(var line in data){
                        SaveOrder(db, line[1],line[2],int.Parse(line[0]));
                        SaveTicket(db,int.Parse(line[0]),line[3],DateTime.Parse(line[4]));                        
                    }
                }
                scope.Complete();
            }
        }

        private int SaveOrder(DBContext db, string firstName, string lastName, int? orderId = null){
             var order = new Order(){                
                First_Name = firstName,
                Last_Name = lastName
            };
            if(orderId.HasValue){
                order.Order_Id = orderId.Value;
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Orders ON"); 
            }                                    
            db.Orders.AddRange(order);       
            db.SaveChanges();        
            if(orderId.HasValue){ 
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Orders OFF");   
            }    
            return order.Order_Id;                    
        }

        private void SaveTicket(DBContext db, int orderId, string ticketNumber, DateTime eventDate){
            var ticket = new Ticket(){
                Order_Id = orderId,
                Ticket_Number = ticketNumber,
                Event_Date = eventDate
            };            
            db.Tickets.AddRange(ticket);
            db.SaveChanges();          
        }

        public bool DeleteOrder(int orderId,int ticketId)
        {
            using(var db = new DBContext()){
                var order = db.Orders.Where(o => o.Order_Id == orderId).FirstOrDefault();
                if(order == null) return false;
                RemoveTicket(orderId,ticketId);
                db.Remove(order);
                db.SaveChanges();            
            }
            return true;
        }

        private void RemoveTicket(int orderId, int ticketId)
        {
            using(var db = new DBContext()){
                var ticket = db.Tickets.Where(o => o.Order_Id == orderId && o.Ticket_Id  == ticketId).FirstOrDefault();
                if(ticket != null){
                    db.Remove(ticket);                 
                }
                db.SaveChanges();  
            }
        }

        internal void SaveOrderTicket(OrderTicketModel orderTicket)
        {
            using(var db = new DBContext()){
                int orderId = SaveOrder(db,orderTicket.FirstName,orderTicket.LastName);
                SaveTicket(db,orderId,orderTicket.TicketNumber,orderTicket.EventDate);
            }
        }

        internal void UpdateOrderTicket(OrderTicketModel orderTicket)
        {
            using(var db = new DBContext()){
                UpdateOrder(db,orderTicket.OrderId,orderTicket.FirstName,orderTicket.LastName);
                UpdateTicket(db,orderTicket.OrderId,orderTicket.TicketNumber,orderTicket.EventDate, orderTicket.TicketId);
            }
        }

        private void UpdateOrder(DBContext db, int orderId, string firstName, string lastName){
            var order = db.Orders.Where(o => o.Order_Id == orderId).FirstOrDefault();
            if(order == null) return;
            order.First_Name = firstName;
            order.Last_Name = lastName;                
            db.SaveChanges();            
        }

        private void UpdateTicket(DBContext db, int orderId,string ticketNumber, DateTime eventDate, int ticketId ){
            var ticket = db.Tickets.Where(o => o.Order_Id == orderId && o.Ticket_Id == ticketId).FirstOrDefault();
            if(ticket != null){
                ticket.Ticket_Number = ticketNumber;
                ticket.Event_Date = eventDate;
                db.SaveChanges();
            }            
        }
    }
}