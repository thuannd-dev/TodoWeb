﻿using Microsoft.EntityFrameworkCore;
using TodoWeb.Application.Dtos.SchoolModel;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.Services.School
{
    public class SchoolService : ISchoolService
    {
        private readonly IApplicationDbContext _context;

        public SchoolService(IApplicationDbContext context)
        {
            _context = context;
        }


        public IEnumerable<SchoolViewModel> GetSchools(int? schoolId)
        {
            var query = _context.School
                .Where(school => school.Status != Constants.Enums.Status.Deleted)
                .AsQueryable();//build leen 1 cau query

            if (schoolId.HasValue)
            {

                query = query.Where(x => x.Id == schoolId);//add theem ddk 
            }

            return query.Select(x => new SchoolViewModel
            {
                Id = x.Id,
                Address = x.Address,
                Name = x.Name,

            }).ToList();//khi minhf chaams to list thi entity framework moi excute cau query 
            //chua to list thif se build tren memory
        }

        public int Post(SchoolViewModel school)
        {
            // kiểm tra xem school id có bị trùng hay không
            var dupID = _context.School.Find(school.Id);
            if (dupID != null || school.Id < 1)
            {
                return -1;
            }
            // lấy school nhờ vào school name
            var existingSchool = _context.School.FirstOrDefault(s => s.Name.Equals(school.Name)); // không dùng where bởi vì tìm ra một list

            if (existingSchool != null)
            {
                return -1;
            }

            var data = new Domains.Entities.School
            {
                Name = school.Name,
                Address = school.Address,
            };
            var state = _context.Entry(data).State;
            _context.School.Add(data);
            //_context.Entry(data).State = EntityState.Added;
            state = _context.Entry(data).State;
            _context.SaveChanges();
            state = _context.Entry(data).State;
            data.Address = "123";
            state = _context.Entry(data).State;
            _context.School.Add(data);
            return data.Id;
        }

        public int Put(SchoolViewModel school)
        {
            var data = _context.School.Find(school.Id);
            //tìm student
            if (data == null || data.Status == Constants.Enums.Status.Deleted)
            {
                return -1;
            }
            var name = school.Name.Split(' ');
            //kiểm tra xem người dùng có đưa đúng tên school
            var existingSchool = _context.School.FirstOrDefault(s => s.Name.Equals(school.Name));//không dùng where bởi vì tìm ra một list
            if (existingSchool != null)
            {
                return -1;
            }
            data.Name = school.Name;
            data.Address = school.Address;
            _context.SaveChanges();
            return data.Id;
        }

        public int Delete(int schoolId)
        {
            var data = _context.School.Find(schoolId);
            if (data == null || data.Status == Constants.Enums.Status.Deleted)
            {
                return -1;
            }
            _context.School.Remove(data);
            _context.SaveChanges();
            return data.Id;
        }

        public SchoolStudentViewModel GetSchoolDetail(int schoolId)
        {
            var school = _context.School.Find(schoolId);
            if(school == null || school.Status == Constants.Enums.Status.Deleted)
            {
                return null;
            }
            _context.Entry(school).Collection(x => x.Students).Load();
            var students = school.Students;

            return new SchoolStudentViewModel
            {
                Id = school.Id,
                Name = school.Name,
                Address = school.Address,
                Students = students.Select(x => new Dtos.StudentModel.StudentViewModel
                {
                    Id = x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    Age = x.Age,
                    Balance = x.Balance,
                    SchoolName = school.Name
                }).ToList()
            };
        }
    }
}
