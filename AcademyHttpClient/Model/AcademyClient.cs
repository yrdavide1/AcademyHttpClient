using AcademyHttpClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AcademyHttpClient
{
    public class AcademyClient
    {
        static HttpClient client = new HttpClient();

        #region Async Methods
        static async Task<List<Student>> GetStudentsAsync(string path)
        {
            List<Student> s = new List<Student>();
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<List<Student>>();
            }

            if (s.Any()) return s;
            return null;
        }

        private static async Task<Student> GetStudentByFullnameAsync(string fullnamePath)
        {
            HttpResponseMessage response = await client.GetAsync(fullnamePath);
            if (response.IsSuccessStatusCode)
            {
                var students = await response.Content.ReadAsAsync<List<Student>>();
                if (students.Any()) return students.First();
            }
            return null;
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
                Console.Write("\tDate of birth (yyyy/MM/dd) --> ");
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

            Console.Write("\tAddress --> ");
            created.Address = Console.ReadLine();

            Console.Write("\tCity --> ");
            created.City = Console.ReadLine();

            do
            {
                Console.Write("\tEmail --> ");
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
                Console.Write("\tPhone number --> ");
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
                Console.Write("\tIs Employee? (y/n) --> ");
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

        static async Task<Student> UpdateStudentAsync(string baseAddress)
        {
            #region old
            //string urlToUpdate;
            //string fullname;
            //do
            //{
            //    Console.Write("Insert firstname and lastname of the student you want to update (separe them with a blank space) --> ");
            //    fullname = Console.ReadLine();

            //    if (fullname.Split(' ').Length == 2)
            //    {
            //        string formattedFullname = string.Join("%20", fullname.Split(' '));
            //        urlToUpdate = $"{baseAddress}name?fullname={formattedFullname}";
            //        break;
            //    }
            //    else Console.WriteLine($"The string you gave as input is {fullname.Split(' ').Length} words long! Please insert a 2 words long string!");
            //} while (true);

            //List<Student> studentList = await GetStudentsAsync(urlToUpdate);
            //if (studentList.Any() && studentList.Count == 1)
            //{
            //    string firstname = studentList.First().Firstname;
            //    string lastname = studentList.First().Lastname;
            //    Student student = StudentInput(firstname, lastname);
            //    HttpResponseMessage response = await client.PutAsJsonAsync(baseAddress, student);
            //    response.EnsureSuccessStatusCode();
            //    var updated = await response.Content.ReadAsAsync<Student>();
            //    return updated.Firstname + " " + updated.Lastname;
            //}

            //Console.WriteLine($"DB error: there are more than one student named {fullname}! There were {studentList.Count} students.");
            //return null;
            #endregion

            string fullname, findByNameUrl;
            Student oldStudent, newStudent;
            HttpResponseMessage response;

            do
            {
                Console.Write("Insert firstname and lastname of the student you want to update (separe them with a blank space) --> ");
                fullname = Console.ReadLine();

                if (fullname.Split(' ').Length == 2)
                {
                    string formattedFullname = string.Join("%20", fullname.Split(' '));
                    findByNameUrl = $"{baseAddress}name?fullname={formattedFullname}";
                    break;
                }
                else Console.WriteLine($"The string you gave as input is {fullname.Split(' ').Length} words long! Please insert a 2 words long string!");
            } while (true);
            oldStudent = await GetStudentByFullnameAsync(findByNameUrl);
            Console.WriteLine((await DeleteStudentAsync(oldStudent.Id, baseAddress)).ToString());

            newStudent = StudentInput();
            response = await client.PostAsJsonAsync(baseAddress, newStudent);
            response.EnsureSuccessStatusCode();

            newStudent = await response.Content.ReadAsAsync<Student>();
            return newStudent;
        }

        static async Task<HttpStatusCode> DeleteStudentAsync(long id, string baseAddress)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{baseAddress}{id}");
            return response.StatusCode;
        }
        #endregion

        static void ShowStudents(List<Student> students)
        {
            foreach (Student student in students)
            {
                string isEmployee;
                if (student.IsEmployee) isEmployee = "Yes";
                else isEmployee = "No";
                Console.WriteLine($"Id:{student.Id}\nFirstname: {student.Firstname}\n" +
                    $"Lastname: {student.Lastname}\nDate of Birth: {student.DateOfBirth}\n" +
                    $"Address: {student.Address}\nCity: {student.City}\n" +
                    $"Email: {student.Email}\nPhone number: {student.PhoneNumber}\n" +
                    $"Is Employee? --> {isEmployee}\n");
            }
        }

        #region Main Async Method
        public static async Task RunAsync()
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
                    "\n\t--> 4 - Delete student;\n\t--> ");
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        Console.WriteLine("Console will close shortly...");
                        System.Threading.Thread.Sleep(1500);
                        break;
                    case "1":
                        Console.WriteLine($"URL --> {client.BaseAddress.ToString() + "student/name"}\n");
                        ShowStudents(await GetStudentsAsync(client.BaseAddress.ToString() + "student/name"));
                        break;
                    case "2":
                        long createdId = await CreateStudentAsync(client.BaseAddress.ToString() + "student");
                        Console.WriteLine("Creating new student...");
                        System.Threading.Thread.Sleep(1500);
                        Console.WriteLine($"Student succesfully created with ID number {createdId}");
                        break;
                    case "3":
                        Student student = await UpdateStudentAsync($"{client.BaseAddress}student/");
                        Console.WriteLine("Updating student...");
                        System.Threading.Thread.Sleep(1500);
                        Console.WriteLine($"Student updated successfully! New ID number is {student.Id}.");
                        break;
                    case "4":
                        Console.Write("Insert the ID of the student you want to delete --> ");
                        long idToDelete = long.Parse(Console.ReadLine());
                        Console.WriteLine("Deleting student...");
                        System.Threading.Thread.Sleep(1500);
                        await DeleteStudentAsync(idToDelete, $"{client.BaseAddress}student/");
                        break;
                    default:
                        Console.WriteLine("Invalid value detected!");
                        break;
                }

                if (choice != "0")
                {
                    Console.WriteLine("\nPress any key...");
                    Console.ReadKey();
                    Console.Clear();
                }
            } while (choice != "0");

        }
        #endregion

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
            if (str.Length == 1)
            {
                c = str.ToLower()[0];
                return true;
            }
            c = '.';
            return false;
        }
        #endregion
        private static Student StudentInput()
        {
            Student created = new Student();

            Console.Write("Input Student obj fields:" +
                "\n\tFirstname --> ");
            created.Firstname = Console.ReadLine();

            Console.Write("\tLastname --> ");
            created.Lastname = Console.ReadLine();

            string date;
            do
            {
                Console.Write("\tDate of birth (yyyy/MM/dd) --> ");
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

            Console.Write("\tAddress --> ");
            created.Address = Console.ReadLine();

            Console.Write("\tCity --> ");
            created.City = Console.ReadLine();

            do
            {
                Console.Write("\tEmail --> ");
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
                Console.Write("\tPhone number --> ");
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
                Console.Write("\tIs Employee? (y/n) --> ");
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

            return created;
        }
    }
}
