using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fort.GamePlay
{
    public class Controller:Actor
    {
        private static Dictionary<string, Type> _controllerTypes =
    typeof(Controller).Assembly.GetTypes()
        .Where(type => typeof(Controller).IsAssignableFrom(type))
        .ToDictionary(type => type.Name);
        private Pawn _pawn;
        public Pawn Pawn
        {
            get { return _pawn; }
            protected set
            {
                _pawn = value;
                PawnChanged(value);
            }
        }

        protected virtual void PawnChanged(Pawn pawn)
        {

        }
        public virtual void PossePawn(Pawn pawn)
        {
            if (pawn != null && pawn != Pawn)
            {
                pawn.Posse(null);
            }
            Pawn = pawn;
            if (pawn != null)
                pawn.Posse(this);
            PawnPossed(pawn);
        }
        public static Controller CreateControllerAndPosse(string controllerClassType, Pawn pawn)
        {
            //string firstOrDefault = typeof(Controller).Assembly.GetTypes().Select(type => type.Name).FirstOrDefault(s => s.StartsWith("SibilYeKoti"));
            return CreateControllerAndPosse(_controllerTypes[controllerClassType], pawn);
        }
        public static Controller CreateControllerAndPosse(Type controllerType, Pawn pawn)
        {
            Controller result = CreateController(controllerType);
            result.PossePawn(pawn);
            return result;
        }
        public static Controller CreateController(string controllerClassType)
        {
            return CreateController(Type.GetType(controllerClassType));
        }
        public static Controller CreateController(Type controllerType)
        {
            GameObject controllerGameObject = new GameObject(controllerType.Name);
            controllerGameObject.AddComponent(controllerType);
            Controller result = (Controller)controllerGameObject.GetComponent(controllerType);
            return result;
        }

        protected virtual void PawnPossed(Pawn pawn)
        {
        }
        public virtual void PawnDied(Controller causer)
        {
        }
    }
}
