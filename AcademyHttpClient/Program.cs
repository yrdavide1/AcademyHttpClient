using Newtonsoft.Json.Linq;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AcademyHttpClient
{
    public class Person
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class Student : Person
    {
        public bool IsEmployee { get; set; }
    }

    public class Program
    {
        static HttpClient client = new HttpClient();

        static void ShowStudents(List<Student> students)
        {
            foreach(Student student in students)
            {
                Console.WriteLine($"Firstname: {student.Firstname}\n" +
                    $"Lastname: {student.Lastname}\nEmail: {student.Email}");
            }
        }

        static async Task<List<Student>> GetStudentsAsync(string path)
        {
            List<Student> s = new List<Student>();
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<List<Student>>();
            }

            return s;
        }

        static async Task<long> CreateStudentAsync(string url)
        {
            #region student_input

            Student created = new Student();

            Console.Write("Input Student obj fields:" +
                "\n\tFirstname --> ");
            created.Firstname = Console.ReadLine();

            Console.Write("\n\tLastname --> ");
            created.Lastname = Console.ReadLine();

            string date;
            do
            {
                Console.Write("\n\tDate of birth (yyyy/MM/dd) --> ");
                date = Console.ReadLine();
                if (date.Split('/').Length == 3)
                {
                    if (date.Split('/')[0].Length == 4
                        && date.Split('/')[1].Length == 2
                        && date.Split('/')[2].Length == 2)
                    {
                        created.DateOfBirth = date;
                        break;
                    }
                }
                else Console.Write("\n\tWrong pattern detected! Please insert following yyyy/MM/dd pattern");

            } while (true);

            Console.Write("\n\tAddress --> ");
            created.Address = Console.ReadLine();

            Console.Write("\n\tCity --> ");
            created.City = Console.ReadLine();

            do
            {
                Console.Write("\n\tEmail --> ");
                string email = Console.ReadLine();
                if (IsValidEmail(email))
                {
                    created.Email = email;
                    break;
                }
                else Console.Write("\n\tWrong email pattern detected! Please try again!");
            } while (true);

            do
            {
                Console.Write("\n\tPhone number --> ");
                string pn = Console.ReadLine();
                if (pn.Length == 10)
                {
                    var isNumeric = long.TryParse(pn, out _);
                    if (isNumeric)
                    {
                        created.PhoneNumber = pn;
                        break;
                    }
                    else Console.WriteLine("\n\tPhone number must be parsable into an integer value! Please try again!");
                }
                else Console.WriteLine("\n\tPhone number must be 10 chars long! Please try again!");
                
            } while (true);

            do
            {
                Console.Write("\n\tIs Employee? (y/n) --> ");
                string ans = Console.ReadLine();
                char c;
                if (IsValidChar(ans, out c))
                {
                    if (c == 'y') created.IsEmployee = true;
                    else created.IsEmployee = false;
                    break;
                }
                else Console.WriteLine("\n\tInvalid answer detected! Please insert y/Y [yes] or n/N [no]");
            } while (true);

            #endregion

            HttpResponseMessage response = await client.PostAsJsonAsync(url, created);
            response.EnsureSuccessStatusCode();

            var createdId = await GetStudentsAsync($"{url}/name?fullname={created.Firstname}");
            return createdId.First().Id;
        }

        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://localhost:44331/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string choice;

            do
            {
                Console.Write("Input your choice:" +
                    "\n\t--> 0 - Stop running console;" +
                    "\n\t--> 1 - List every student;" +
                    "\n\t--> 2 - Create new student;" +
                    "\n\t--> 3 - Update student;" +
                    "\n\t--> 4 - Delete student;" +
                    "\n\t--> 5 - Run automatic snippet;\n\t--> ");
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        Console.WriteLine("Console will close shortly...");
                        System.Threading.Thread.Sleep(3000);
                        break;
                    case "1":
                        Console.WriteLine($"URL --> {client.BaseAddress.ToString() + "student/name"}\n");
                        ShowStudents(await GetStudentsAsync(client.BaseAddress.ToString() + "student/name"));
                        break;
                    case "2":
                        Console.WriteLine("Creating new student...");
                        System.Threading.Thread.Sleep(1500);
                        long createdId = await CreateStudentAsync(client.BaseAddress.ToString() + "student");
                        Console.WriteLine($"Student succesfully created with ID number {createdId}");
                        break;
                }

                if(choice != "0")
                {
                    Console.WriteLine("\nPress any key...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }while (choice != "0");
            
        }

        #region CreateStudent checks
        private static bool IsValidEmail(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidChar(string str, out char c)
        {
            if(str.Length == 1)
            {
                c = str.ToLower()[0];
                return true;
            }
            c = '.';
            return false;
        }
        #endregion
    }
}


