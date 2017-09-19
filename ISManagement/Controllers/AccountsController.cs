using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ISManagement.Models;
using ISManagement.ViewModels;

namespace ISManagement.Controllers
{
    public class AccountsController : Controller
    {
        private masterEntities db = new masterEntities();

        // GET: Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Person);
            return View(accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            else
            {
                var transactions = account.Transactions.ToList();
                var viewModel = new AccountTransactions
                {
                    Account = account,
                    Transactions = transactions
                };

                return View(viewModel);
            }
            //return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "name");
            return View();
        }

        // GET: Accounts/Create/5
        [HttpGet]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            else
            {
                account.account_number = "";
                account.outstanding_balance = 0;
            }
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "name", account.PersonId);
            return View(account);
        }



        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PersonId,account_number,outstanding_balance")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Details", "Persons", new { id = account.PersonId });
            }

            ViewBag.PersonId = new SelectList(db.Persons, "Id", "name", account.PersonId);
            return View(account);
        }

        
        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "name", account.PersonId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PersonId,account_number,outstanding_balance")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Persons", new { id = account.PersonId });
            }
            ViewBag.PersonId = new SelectList(db.Persons, "Id", "name", account.PersonId);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
