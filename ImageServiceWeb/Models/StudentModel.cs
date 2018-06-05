using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace ImageServiceWeb.Models
{
    public static class StudentModel
    {
        public static List<Student> GetStudentList(string path)
        {
            List<Student> students = new List<Student>();

            Stack<string> f_names = new Stack<string>();
            Stack<string> l_names = new Stack<string>();
            Stack<string> ids = new Stack<string>();
            Stack<string> groups = new Stack<string>();
            int i = 0;

            XmlReader reader = XmlReader.Create(System.Web.Hosting.HostingEnvironment.MapPath(@"~/" + path));
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name.ToString())
                    {
                        case "FirstName":
                            i++;
                            f_names.Push(reader.ReadString());
                            break;
                        case "LastName":
                            l_names.Push(reader.ReadString());
                            break;
                        case "ID":
                            ids.Push(reader.ReadString());
                            break;
                        case "Group":
                            groups.Push(reader.ReadString());
                            break;
                    }
                }
            }
            reader.Close();

            for (; i > 0; i--)
            {
                students.Add(new Student()
                {
                    FirstName = f_names.Pop(),
                    LastName = l_names.Pop(),
                    ID = ids.Pop(),
                    Group = groups.Pop()
                });
            }

            return students;
        }
    }
}