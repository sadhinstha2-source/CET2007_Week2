using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CET2007_Week2
{

    // ========== Inheritance ==========
    class Person
    {
        public string Name { get; set; }

        // Constructor
        public Person(string name)
        {
            Name = name;
        }

        public void Introduce()
        {
            Console.WriteLine($"Hi, I am {Name}.");
        }
    }

    // Student inherits from Person
    class Student : Person
    {
        public string StudentId { get; private set; }

        // Calls base class constructor
        public Student(string name, string studentId) : base(name)
        {
            StudentId = studentId;
        }

        public void ShowStudentCard()
        {
            Console.WriteLine($"Student ID: {StudentId}");
        }
    }

    // ========== Interfaces ==========
    interface IAuthService
    {
        // Contract: any AuthService must implement this method
        bool ValidateUser(string username, string password);
    }

    // Simple authentication service using arrays
    class AuthService : IAuthService
    {
        private string[] usernames = { "alice", "bob" };
        private string[] passwords = { "1234", "letmein" };

        public bool ValidateUser(string username, string password)
        {
            // Loop through arrays to check credentials
            for (int i = 0; i < usernames.Length; i++)
            {
                if (username.ToLower() == usernames[i] && password == passwords[i])
                {
                    return true;
                }
            }
            return false;
        }
    }

    // ========== Composition ==========
    class Syllabus
    {
        public string Overview { get; }
        private string[] Topics;

        public Syllabus(string overview, string[] topics)
        {
            Overview = overview;
            Topics = topics; // owned by the Course
        }

        public void Print()
        {
            Console.WriteLine($"Syllabus: {Overview}");
            Console.WriteLine("Topics:");
            for (int i = 0; i < Topics.Length; i++)
            {
                Console.WriteLine($"  - {Topics[i]}");
            }
        }
    }

    class Course
    {
        public string Code { get; }
        public string Title { get; }
        public Syllabus Syllabus { get; }

        public Course(string code, string title, Syllabus syllabus)
        {
            Code = code;
            Title = title;
            Syllabus = syllabus; // composition
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"{Code} - {Title}");
        }
    }

    // ========== Aggregation ==========
    class University
    {
        public string Name { get; }

        // Arrays to hold students and courses
        private Student[] students = new Student[100];
        private Course[] courses = new Course[100];
        private int studentCount = 0;
        private int courseCount = 0;

        public University(string name)
        {
            Name = name;
        }

        // Register student to course (aggregation)
        public void Register(Student s, Course c)
        {
            // Add student if not already added
            bool studentExists = false;
            for (int i = 0; i < studentCount; i++)
            {
                if (students[i] == s) studentExists = true;
            }
            if (!studentExists && studentCount < students.Length)
            {
                students[studentCount] = s;
                studentCount++;
            }

            // Add course if not already added
            bool courseExists = false;
            for (int i = 0; i < courseCount; i++)
            {
                if (courses[i] == c) courseExists = true;
            }
            if (!courseExists && courseCount < courses.Length)
            {
                courses[courseCount] = c;
                courseCount++;
            }

            Console.WriteLine($"Registered {s.Name} ({s.StudentId}) to {c.Code} - {c.Title}");
        }

        public void PrintSummary()
        {
            Console.WriteLine($"\n=== {Name} Summary ===");

            Console.WriteLine("Students:");
            for (int i = 0; i < studentCount; i++)
            {
                Console.WriteLine($"  - {students[i].Name} ({students[i].StudentId})");
            }

            Console.WriteLine("\nCourses:");
            for (int i = 0; i < courseCount; i++)
            {
                courses[i].DisplayInfo();
            }
        }
    }

    // ========== "System" (Sequence Diagram idea) ==========
    class StudentPortal
    {
        private readonly IAuthService _auth;
        private readonly University _uni;

        public StudentPortal(IAuthService auth, University uni)
        {
            _auth = auth;
            _uni = uni;
        }

        public bool EnterCredentials(string username, string password)
        {
            return _auth.ValidateUser(username, password);
        }

        public void ShowLoginResult(bool success)
        {
            Console.WriteLine(success ? "Login successful." : "Login failed.");
        }

        public void RegisterForCourse(Student s, Course c)
        {
            _uni.Register(s, c);
        }
    }

    // ========== Main Program ==========
    class Program
    {
        static void Main()
         {
            var uni = new University("University of Sunderland");

            // Composition: Course has its own Syllabus
            var syllabus = new Syllabus(
                "Object-oriented programming and design foundations.",
                new string[]
                {
                    "Classes & Objects",
                    "Constructors",
                    "Inheritance vs. Interfaces",
                    "UML (Aggregation, Composition, Sequence Diagrams)",
                    "Version Control (Git & GitHub)"
                });

            var course = new Course("CET2007", "Systems Design and Development", syllabus);

            IAuthService auth = new AuthService();
            var portal = new StudentPortal(auth, uni);

            // === Interactive Part ===
            Console.WriteLine("=== Student Login ===");
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            Console.Write("Enter your student ID: ");
            string id = Console.ReadLine();

            Console.Write("Enter username (e.g., alice): ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            // Inheritance: Student inherits from Person
            var student = new Student(name, id);
            student.Introduce();
            student.ShowStudentCard();

            // Sequence: Student → Portal → Auth → Portal → Student
            Console.WriteLine("\n-- Login Flow --");
            bool ok = portal.EnterCredentials(username, password);
            portal.ShowLoginResult(ok);

            // If login is ok, register the student to the course
            if (ok)
            {
                portal.RegisterForCourse(student, course);

                Console.WriteLine("\n-- Course & Syllabus --");
                course.DisplayInfo();
                course.Syllabus.Print();
            }

            // Aggregation: University knows all Students & Courses
            uni.PrintSummary();

            Console.WriteLine("\n(End of Week 2 demo)");
        }
    }

}
