// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;

public class Student
{
    public int Id { get; set; }

    public string Name { get; set; }
    public int Age {  get; set; }

    public List<Group> Groups { get; set; } = new();
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IEnumerable<Student> Students { get; set;}
}

public class ApplicationContext : DbContext
{
    public DbSet<Student> Students { get; set; }

    public DbSet<Group> Groups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=testdb;Trusted_Connection=True;TrustServerCertificate=True");

    }

}

public class DatabaseService
{
    public void InitialData()
    {
        using(ApplicationContext db=new ApplicationContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Group[] groups = new Group[]
            {
                new Group{ Name="Math"},
                new Group{ Name="Chemistry"},
                new Group{ Name="Physics"}

            };
            Student[] students = new Student[]
            {
                new Student{Name="Mira",Age=17},
                new Student{Name="Dima",Age=18},
                new Student{Name="Lera",Age=16},
                new Student{Name="Julia",Age=18},
                new Student{Name="Max",Age=15},


            };

            students[0].Groups.AddRange(new Group[] { groups[0], groups[2] });
            students[1].Groups.AddRange(new Group[] { groups[0], groups[2] });
            students[2].Groups.Add( groups[1]);
            students[3].Groups.Add(groups[0]);

            db.Groups.AddRange(groups);
            db.Students.AddRange(students);
            db.SaveChanges();

        }
    }

    public void AddStudentWithGroups(Student student, List<int> groupIds)
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            foreach (var groupId in groupIds)
            {
                var group=db.Groups.FirstOrDefault(e=>e.Id==groupId);
                if (group != null)
                {
                    student.Groups.Add(group);
                }
            }
            db.Students.Add(student);
            db.SaveChanges();
        }
    }

    public List<Group> GetGroupsByStudent(int studentId)
    {

        using (ApplicationContext db = new ApplicationContext())
        {
            var student =db.Students.Include(e=>e.Groups).FirstOrDefault(e=>e.Id== studentId);
            return student.Groups.ToList();
        }
    }

    public List<Student> GetStudentsByGroup(int groupId)
    {

        using (ApplicationContext db = new ApplicationContext())
        {
            var group = db.Groups.Include(e => e.Students).FirstOrDefault(e => e.Id == groupId);
            return group.Students.ToList();
        }
    }

}


class Program
{
    public static void Main()
    {
        DatabaseService db= new DatabaseService();
        db.InitialData();

        db.AddStudentWithGroups(new Student { Name="Jo",Age=20},new List<int> {1,2});

        List<Group> groups=db.GetGroupsByStudent(1);

        List<Student> students=db.GetStudentsByGroup(1);
    }
}