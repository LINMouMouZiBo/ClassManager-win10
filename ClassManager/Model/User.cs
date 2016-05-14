using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassManager.Model
{
    public class User : INotifyPropertyChanged
    {
        public string _id;
        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("ID");
            }
        }

        public string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string account;
        public string Account
        {
            get { return account; }
            set
            {
                account = value;
                NotifyPropertyChanged("Account");
            }
        }

        public string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                NotifyPropertyChanged("Password");
            }
        }

        public string student_id;
        public string StudentId
        {
            get { return student_id; }
            set
            {
                student_id = value;
                NotifyPropertyChanged("StudentId");
            }
        }

        public string gender;
        public string Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                NotifyPropertyChanged("Gender");
            }
        }

        public string image;
        public string Image
        {
            get { return image; }
            set
            {
                image = value;
                NotifyPropertyChanged("Image");
            }
        }

        public string phone;
        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        public string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                NotifyPropertyChanged("Email");
            }
        }

        public string qq;
        public string QQ
        {
            get { return qq; }
            set
            {
                qq = value;
                NotifyPropertyChanged("QQ");
            }
        }

        public string wechat;
        public string Wechat
        {
            get { return wechat; }
            set
            {
                wechat = value;
                NotifyPropertyChanged("Wechat");
            }
        }

        public string position;
        public string Position
        {
            get { return position; }
            set
            {
                position = value;
                NotifyPropertyChanged("Position");
            }
        }

        public ObservableCollection<UserHomework> homeworks;
        public ObservableCollection<UserHomework> Homeworks { get { return homeworks; } }
        public ObservableCollection<Relationship> relationships;
        public ObservableCollection<Relationship> Relationships { get { return relationships; } }

        public User()
        {
            _id = "";
            name = "";
            account = "";
            password = "";
            student_id = "";
            gender = "female";
            image = "";
            email = "";
            phone = "";
            qq = "";
            wechat = "";
			position = "";
			homeworks = new ObservableCollection<UserHomework>();
            relationships = new ObservableCollection<Relationship>();
        }

        public void DeepCopy(User u)
        {
            ID = u.ID;
            Name = u.Name;
            Account = u.Account;
            Password = u.Password;
            StudentId = u.StudentId;
            Gender = u.Gender;
            Image = u.Image;
            Email = u.Email;
            Phone = u.Phone;
            QQ = u.QQ;
            Wechat = u.Wechat;
            Position = u.Position;

            Homeworks.Clear();
            foreach (UserHomework item in u.Homeworks)
            {
                Homeworks.Add(item);
            }

            Relationships.Clear();
            foreach (Relationship item in u.Relationships)
            {
                Relationships.Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // NotifyPropertyChanged will raise the PropertyChanged event passing the
        // source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Relationship
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string account { get; set; }
        public string image { get; set; }
        public string position { get; set; }

        public Relationship()
        {
            _id = "";
            name = "";
            account = "";
            image = "";
            position = "";
        }
    }

    public class UserHomework
    {

        public string _id { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public string join_on { get; set; }
        public string deadline { get; set; }
        public string account { get; set; }
        public bool unlook { get; set; }
        public bool uncomplish { get; set; }
        public UserHomework()
        {
            _id = "";
            name = "";
            content = "";
            join_on = "";
            deadline = "";
            account = "";
            unlook = true;
            uncomplish = true;
        }
    }
}
