using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face.Contract;
using ServiceHelpers;
using System;
using System.Collections.Generic;

namespace TotemInteligente.Models
{
    public enum TypeOfChat
    {
        None,
        Text,
        Voice,
        Libras
    }

    public class UserData
    {
        public Face Face { get; set; }

        public Emotion Emotion { get; set; }

        public IdentifiedPerson Person { get; set; }
        public TypeOfChat Chat { get; set; }
        public DateTime Datetime { get; set; }
    }
}
