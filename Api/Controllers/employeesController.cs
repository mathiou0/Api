using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Api;
using Newtonsoft.Json;
namespace Api.Controllers
{
    public class employeesController : ApiController
    {
        private Employees_DBEntities db = new Employees_DBEntities();


      
        // GET: api/employees
        public IEnumerable<employee> Getemployees()
        {
            return db.employees.ToArray();
        }

        

        // GET: api/employees/5
        [ResponseType(typeof(employee))]
        public async Task<IHttpActionResult> Getemployee(int id)
        {
            employee employee = await db.employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }


        // GET: api/employees/5
        [ResponseType(typeof(employee))]
        public async Task<IHttpActionResult> GetemployeeByName(string name)
        {

            employee employee = await (db.employees.Where(u => u.First_Name == name).FirstOrDefaultAsync<employee>());

            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }



        // PUT: api/employees/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putemployee(int id, employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.employee_id)
            {
                return BadRequest();
            }

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/employees
        [ResponseType(typeof(employee))]
        public async Task<IHttpActionResult> Postemployee(employee employee)
        {
            if(!employee.birthdate.HasValue)
            {
                employee.birthdate = DateTime.Today;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //employee.birthdate= JsonConvert.DeserializeObject<DateTime>(employee.birthdate.ToString());

            if (!String.IsNullOrEmpty(employee.First_Name) && !String.IsNullOrEmpty(employee.Last_Name) )
            { 
                db.employees.Add(employee);
                await db.SaveChangesAsync();
               
                return CreatedAtRoute("DefaultApi", new { id = employee.employee_id }, employee);
            }
            else
            {
                
                return ResponseMessage( Request.CreateResponse(
                                            HttpStatusCode.BadRequest,"Validation Error"));
            }
            
        }

        // DELETE: api/employees/5
        [ResponseType(typeof(employee))]
        public async Task<IHttpActionResult> Deleteemployee(int id)
        {
            employee employee = await db.employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();

            }
           
            salary salary_move = await (db.salaries.Where(u => u.employee_id == employee.employee_id).FirstOrDefaultAsync<salary>()); ;
            await db.Database.ExecuteSqlCommandAsync("Delete from salaries where employee_id="+ employee.employee_id);
            db.employees.Remove(employee);
           
            await db.SaveChangesAsync();
            return Ok(employee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool employeeExists(int id)
        {
            return db.employees.Count(e => e.employee_id == id) > 0;
        }
    }
}