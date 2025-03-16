public partial class GameContext
{
    public class RuntimeContext
    {
        public Inventory inventory;

        public struct Inventory
        {
            public UI_ItemSlot heldSlot;
            public bool isMouseHolding;
        }
    }
}