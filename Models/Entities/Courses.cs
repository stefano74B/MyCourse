﻿using System;
using System.Collections.Generic;
using MyCourse.Models.ValueObjects;

namespace MyCourse.Models.Entities
{
    public partial class Course
    {
        public Course(string title, string author)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Il corso deve avere un titolo");
            }
            if(string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Il corso deve avere un autore");
            }

            Title = title;
            Author = author;

            Lessons = new HashSet<Lesson>();
        }

        public long Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Author { get; private set; }
        public string Email { get; private set; }
        public double Rating { get; private set; }
        public Money FullPrice { get; private set; }
        public Money CurrentPrice { get; private set; }

        //Ho reso privata la set e ora creo un metodo personalizzato per cambiare la proprietà
        public void ChangeTitle(string newTitle)
        {
            if(string.IsNullOrWhiteSpace(newTitle))
            {
                throw new ArgumentException("Il corso deve avere un titolo");
            }
        }

        public void ChangePrices(Money newFullPrice, Money newDiscountPrice)
        {
            if (newFullPrice == null || newDiscountPrice = null)
            {
                throw new ArgumentException("I prezzi non possono essere nulli");
            }
            if (newFullPrice.Currency != newDiscountPrice.Currency)
            {
                throw new ArgumentException("Le divise non coincidono");
            }
            if (newFullPrice.Amount < newDiscountPrice.Amount)
            {
                throw new ArgumentException("Il prezzo pieno non può essere minore del prezzo scontato");
            }
            FullPrice = newFullPrice;
            CurrentPrice = newDiscountPrice;
        }

        public virtual ICollection<Lesson> Lessons { get; private set; }
    }
}
