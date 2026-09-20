using UnityEngine;

public class BlocksManager : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int numberOfRows = 5;
    [SerializeField] private int numberOfColumns = 10;

    private int numberOfBlocks;

    private float blockHeight;
    private float blockWidth;

    //Block Behaviour is the component type on the block prefab itself
    private BlockBehaviour[] blocks;

    private void Awake()
    {
        numberOfBlocks = numberOfRows * numberOfColumns;

        blocks = new BlockBehaviour[numberOfBlocks];

        blockWidth = blockPrefab.transform.localScale.x;
        blockHeight = blockPrefab.transform.localScale.y;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (BlockBehaviour.onDestroyedBlock != null)
        {
            BlockBehaviour.onDestroyedBlock.AddListener(OnDestroyedBlock);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBlocks()
    {
        int i = 0;

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int col = 0; col < numberOfColumns; col++)
            {
                float x = (col - (numberOfColumns - 1) * 0.5f) * blockWidth;
                float y = (row - (numberOfRows - 1) * 0.5f) * blockHeight;

                GameObject block = Instantiate(blockPrefab, transform);
                block.transform.localPosition = new Vector3(x, y, 0f);

                blocks[i++] = block.GetComponent<BlockBehaviour>();
            }
        }
    }

    private void OnDestroyedBlock()
    {
        numberOfBlocks--;   

        if (numberOfBlocks == 0)
        {
            GameManager.instance.GameOver();
        }
    }
}
