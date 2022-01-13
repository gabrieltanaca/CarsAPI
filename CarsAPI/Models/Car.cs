using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarsAPI.Models
{
    public class Car
    {
        public Car(string model, string marca, int year, double saleValue)
        {
            Id = Guid.NewGuid().ToString();
            Model = model;
            Marca = marca;
            Year = year;
            SaleValue = saleValue;
            SoldValue = null;
            Discount = null;
            Sold = false;
            CreationDate = DateTime.Now;
            SaleDate = null;
        }

        public string Id { get; private set; }

        public string Model { get; private set; }

        public string Marca { get; private set; }

        public int Year { get; private set; }

        public double SaleValue { get; private set; }

        public double? SoldValue { get; private set; }

        public double? Discount { get; private set; }

        public bool Sold { get; private set; }

        public DateTime CreationDate { get; private set; }

        public DateTime? SaleDate { get; private set; }

        public void UpdateCarModel(string model, string marca, int year, double saleValue)
        {
            Model = model;
            Marca = marca;
            Year = year;
            SaleValue = saleValue;
        }

        public void SoldCar(double soldValue, double discount)
        {
            Sold = true;
            SaleDate = Sold ? DateTime.Now : null;
            SoldValue = Sold ? soldValue : null;
            Discount = Sold & discount >= 1 ? discount : 0;
        }

    }
}
