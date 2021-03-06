﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AzureLog.Web.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace AzureLog.Web.Controllers
{
    [Authorize]
    public class StorageAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StorageAccounts
        public async Task<ActionResult> Index()
        {
            var email = User.Identity.Name;
            var list = await db.StorageAccounts.Where(s => s.UserEmail == email).ToListAsync();
            return View(list);
        }

        // GET: StorageAccounts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var email = User.Identity.Name;
            var storageAccount = await db.StorageAccounts.FirstOrDefaultAsync(s => s.Id == id && s.UserEmail == email);
            if (storageAccount == null)
            {
                return HttpNotFound();
            }

            var cloudStorageAccount = new CloudStorageAccount(new StorageCredentials(storageAccount.AccountName, storageAccount.Key), true);
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            var tables = await tableClient.ListTablesAsync();

            var vm = new StorageAccountDetailViewModel
            {
                Id = storageAccount.Id,
                Account = storageAccount.AccountName,
                Key = storageAccount.Key,
                Tables = tables.Select(ct => ct.Name).ToList()
            };

            return View(vm);
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
        public async Task<ActionResult> Create([Bind(Include = "Id,AccountName,Key")] StorageAccount account)
        {
            if (ModelState.IsValid)
            {
                if (await VerifyStorageAccount(account.AccountName, account.Key))
                {
                    account.UserEmail = User.Identity.Name;
                    db.StorageAccounts.Add(account);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("*", "Counldn't access to storage account.");
                }
            }

            return View(account);
        }

        // GET: StorageAccounts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var email = User.Identity.Name;
            var storageAccount = await db.StorageAccounts.FirstOrDefaultAsync(s => s.Id == id && s.UserEmail == email);
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,AccountName,Key")] StorageAccount account)
        {
            if (ModelState.IsValid)
            {
                if (await VerifyStorageAccount(account.AccountName, account.Key))
                {
                    db.Entry(account).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("*", "Counldn't access to storage account.");
                }
            }
            return View(account);
        }

        // GET: StorageAccounts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var email = User.Identity.Name;
            var storageAccount = await db.StorageAccounts.FirstOrDefaultAsync(s => s.Id == id && s.UserEmail == email);
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
            var email = User.Identity.Name;
            var storageAccount = await db.StorageAccounts.FirstOrDefaultAsync(s => s.Id == id && s.UserEmail == email);
            db.StorageAccounts.Remove(storageAccount);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Route("storageaccounts/{id:int}/tables/{table}")]
        public async Task<ActionResult> Table(int id, string table, DateTime? from, DateTime? to, string regex)
        {
            var email = User.Identity.Name;
            var storageAccount = await db.StorageAccounts.FirstOrDefaultAsync(s => s.Id == id && s.UserEmail == email);
            if (storageAccount == null)
            {
                return HttpNotFound();
            }

            var cloudStorageAccount = new CloudStorageAccount(new StorageCredentials(storageAccount.AccountName, storageAccount.Key), true);
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            var cloudTable = tableClient.GetTableReference(table);
            var f = from ?? DateTime.UtcNow.AddDays(-1);
            var t = to ?? DateTime.UtcNow.AddDays(1);

            var query = new QueryLog
            {
                Table = cloudTable,
                From = f,
                To = t,
                Text = regex,
            };

            var logs = await query.ExecuteAsync();
            var vm = new SearchLogResultViewModel
            {
                Id = id,
                Table = table,
                From = f,
                To = t,
                Regex = regex,
                Results = logs,
            };
            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private async Task<bool> VerifyStorageAccount(string account, string key)
        {
            try
            {
                var storageAccount = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(account, key), true);
                var blob = storageAccount.CreateCloudBlobClient();
                var _ = await blob.ListContainersSegmentedAsync(null);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
