using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AzureLog.Web.Models;

namespace AzureLog.Web.Controllers
{
    public class StorageAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StorageAccounts
        public async Task<ActionResult> Index()
        {
            var list = await db.StorageAccounts.ToListAsync();
            return View(list);
        }

        // GET: StorageAccounts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StorageAccount storageAccount = await db.StorageAccounts.FindAsync(id);
            if (storageAccount == null)
            {
                return HttpNotFound();
            }
            return View(storageAccount);
        }

        // GET: StorageAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StorageAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AccountName,Key")] StorageAccount storageAccount)
        {
            if (ModelState.IsValid)
            {
                db.StorageAccounts.Add(storageAccount);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(storageAccount);
        }

        // GET: StorageAccounts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StorageAccount storageAccount = await db.StorageAccounts.FindAsync(id);
            if (storageAccount == null)
            {
                return HttpNotFound();
            }
            return View(storageAccount);
        }

        // POST: StorageAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AccountName,Key")] StorageAccount storageAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(storageAccount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(storageAccount);
        }

        // GET: StorageAccounts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StorageAccount storageAccount = await db.StorageAccounts.FindAsync(id);
            if (storageAccount == null)
            {
                return HttpNotFound();
            }
            return View(storageAccount);
        }

        // POST: StorageAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            StorageAccount storageAccount = await db.StorageAccounts.FindAsync(id);
            db.StorageAccounts.Remove(storageAccount);
            await db.SaveChangesAsync();
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
