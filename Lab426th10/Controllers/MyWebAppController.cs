using Lab426th10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab426th10.Controllers
{
    public class MyWebAppController : Controller
    {
        private StudentManagementContext db;

        public MyWebAppController(StudentManagementContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var learn = db.Learners.Include(m => m.Major).ToList();
            return View(learn);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.MajorId = new SelectList(db.Majors, "MajorId", "MajorName");
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("FirstMidName,LastName,MajorId,EnrollmentDate")] Learner learner)
        {
            if (ModelState.IsValid)
            {
                db.Learners.Add(learner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, tạo lại SelectList và giữ lại giá trị người dùng đã chọn (learner.MajorId)
            ViewBag.MajorId = new SelectList(db.Majors, "MajorId", "MajorName", learner.MajorId);
            return View(learner);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == null || db.Learners == null)
            {
                return NotFound();
            }
            var learner = db.Learners.Find(id);
            if (learner == null)
            {
                return NotFound();
            }
            ViewBag.MajorId = new SelectList(db.Majors, "MajorId", "MajorName", learner.MajorId);
            return View(learner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("LearnerId,FirstMidName,LastName,MajorId,EnrollmentDate")] Learner learner)
        {
            if (id != learner.LearnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(learner);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!db.Learners.Any(e => e.LearnerId == learner.LearnerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MajorId = new SelectList(db.Majors, "MajorId", "MajorName", learner.MajorId);
            return View(learner);
        }
    }
}