using UnityEngine;

    public class Block : MonoBehaviour
    {
        [Header("Block Properties")]
        public Vector2Int gridPosition;
        public bool isIndestructible = true; // Blocks cannot be destroyed
        
        [Header("Visual")] 
        public SpriteRenderer blockRenderer;
        public Color defaultColor = Color.gray;
        public Color highlightColor = Color.yellow;
        
        private bool isHighlighted = false;
        
        void Awake()
        {
            // Set default color
            if (blockRenderer != null)
            {
                blockRenderer.color = defaultColor;
            }
        }
        
        public void SetGridPosition(Vector2Int position)
        {
            gridPosition = position;
        }
        
        public void Highlight()
        {
            if (blockRenderer != null && !isHighlighted)
            {
                blockRenderer.color = highlightColor;
                isHighlighted = true;
            }
        }
        
        public void Unhighlight()
        {
            if (blockRenderer != null && isHighlighted)
            {
                blockRenderer.color = defaultColor;
                isHighlighted = false;
            }
        }
        
        public void SetColor(Color color)
        {
            if (blockRenderer != null)
            {
                blockRenderer.color = color;
            }
        }
        
        public bool CanPlaceOn()
        {
            // Blocks can have towers or monkeys placed on them
            return true;
        }
        
        public bool IsWalkable()
        {
            // Blocks are walkable for monkeys and enemies
            return true;
        }
        
        public bool IsFlyable()
        {
            // Flying enemies can pass through blocks
            return true;
        }
        
        void OnMouseEnter()
        {
            Highlight();
        }
        
        void OnMouseExit()
        {
            Unhighlight();
        }
        
        void OnMouseDown()
        {
            // Handle block selection for card placement
            // This will be handled by the card system
        }
    }
