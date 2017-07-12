namespace Fort.GamePlay
{
    public class Pawn:Actor
    {
        public Controller Controller { get; private set; }
        public void Posse(Controller controller)
        {
            Controller = controller;
            Possed(controller);
        }

        protected virtual void Possed(Controller controller)
        {
        }
        public virtual void InventoryInitialized()
        {

        }
    }
}
