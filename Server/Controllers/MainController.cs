using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheVoid.Models.Manager;
using TheVoid.Models.ResponseTypes;

namespace TheVoid
{

    [Route("Main")]
    public class MainController: Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private OrderTicketManager ordenTicketMan;

        public MainController(IHostingEnvironment hostingEnvironment){
            _hostingEnvironment = hostingEnvironment;
            ordenTicketMan = new OrderTicketManager();
        }

        [HttpGet("Index")]
        public IActionResult Index(){
            return Ok("MainController Works!");
        }

        [HttpGet("OrderTickets")]
        [ProducesResponseType(typeof(IEnumerable<OrderTicketModel>), StatusCodes.Status200OK)]
        public IActionResult GetOrderTickets(){
            try
            {                
                return Ok(ordenTicketMan.GetAllOrderTickets());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("UploadFile")]
        [ProducesResponseType(typeof(IEnumerable<OrderTicketModel>), StatusCodes.Status200OK)]
        public IActionResult UploadFile(){
            try
            {                
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot","Uploads");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    if(!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    var csvLines = new List<string[]>();
                    using(var reader = new StreamReader(fullPath))
                    {                        
                        while (!reader.EndOfStream)
                        {                               
                            var line = reader.ReadLine().Split(',');
                            csvLines.Add(line);
                        }
                    }
                    ordenTicketMan.ImportCSVData(csvLines);
                    return Ok(ordenTicketMan.GetAllOrderTickets());
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{orderId}/{ticketId}")]
        public IActionResult Delete(int orderId, int ticketId){
            try{
                if(!ordenTicketMan.DeleteOrder(orderId,ticketId)) return NotFound();
                return Ok(ordenTicketMan.GetAllOrderTickets()); 
            }catch(Exception ex){
                return StatusCode((int)HttpStatusCode.InternalServerError,ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderTicketModel orderTicket ){
            try{
               if(!ModelState.IsValid) return BadRequest();
               if(orderTicket.OrderId == 0) ordenTicketMan.SaveOrderTicket(orderTicket);
               else ordenTicketMan.UpdateOrderTicket(orderTicket);
               return Ok(ordenTicketMan.GetAllOrderTickets());
            }catch(Exception ex){
                return StatusCode((int)HttpStatusCode.InternalServerError,ex.Message);
            }
        }

    }

}