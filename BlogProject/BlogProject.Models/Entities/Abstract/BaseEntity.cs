﻿using BlogProject.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogProject.Models.Entities.Abstract
{
    public abstract class BaseEntity
    {
        // Entity sınıflarının ortak propertyleri tanımlanır. Bu class diğer entity classlarına kalıtım verecektir.

        // ID required
        public int ID { get; set; }

        // CreateDate propfull olarak tanımlanarak default değer olarak şimdiki zaman atanır. Encapsulation özelliği ile yazılır.
        private DateTime _createDate = DateTime.Now;

        public DateTime CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; }
        }

        // ModefiedDate Nullable
        public DateTime? ModifiedDate { get; set; }

        // RemovedTime Nullable
        public DateTime? RemovedDate { get; set; }

        // Statu propfull olarak tanımlanarak default değer olarak Statu.Passive atanır. Admin onayı sonrası statusu active e döner
        private Statu _statu = Statu.Passive;

        public Statu Statu
        {
            get { return _statu; }
            set { _statu = value; }
        }

        // Admin onayı için boolen tipinde bir property tanımlanır. Default olarak ilk oluşturulduğunda kontrol edilmemiş durumdadır.

        private AdminCheck _adminChecked = AdminCheck.Waiting;
        public AdminCheck AdminCheck
        {
            get { return _adminChecked; }
            set { _adminChecked = value; }
        }

    }
}
