using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=HGol5qhqjOE&ab_channel=FreedomCoding
//https://www.youtube.com/watch?v=Josw0x2geuQ&ab_channel=FreedomCoding
//https://forum.unity.com/threads/how-to-get-the-button-gameobject-when-it-is-clicked.447617/

public class PlayerInventory : MonoBehaviour
{
    [Header("General")]

    public List<itemType> inventoryList;
    public int selectedItem;

    [Space(20)]
    [Header("Item Prefabs")]
    public GameObject test_1;
    public GameObject test_2;
    public GameObject test_3;

    [Space(20)]
    [Header("UI")]
    [SerializeField] private Transform itemSpritesParent; //parent object for the item sprites
    [SerializeField] private Transform itemSelectedIndicatorsParent; //parent object for the selection indicator
    [SerializeField] private List<Image> itemSprites; //list of item sprite children
    [SerializeField] private List<Image> itemSelectedIndicators; //list of selection indicator children
    [SerializeField] Sprite emptySlotSprite;

    [SerializeField] private Camera mainCamera;

    private Dictionary<itemType, GameObject> itemSetActive = new Dictionary<itemType, GameObject>() { };

    void Awake()
    {
        itemSetActive.Add(itemType.test1, test_1);
        itemSetActive.Add(itemType.test2, test_2);
        itemSetActive.Add(itemType.test3, test_3);

        //grabs the inventory images without having to drag them into the inspector, scalable
        foreach (Transform child in itemSpritesParent.transform)
        {
            itemSprites.Add(child.gameObject.GetComponent<Image>());
        }
        foreach (Transform child in itemSelectedIndicatorsParent.transform)
        {
            itemSelectedIndicators.Add(child.gameObject.GetComponent<Image>());
        }
        //UpdateInventoryUI();
    }

    void Update()
    {
        //raycast when clicked
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            //add object to inventory if it has the ItemPickable script component and it will be destroyed
            if (Physics.Raycast(ray, out hitInfo))
            {
                IPickable item = hitInfo.collider.GetComponent<IPickable>();
                if (item != null)
                {
                    inventoryList.Add(hitInfo.collider.GetComponent<ItemPickable>().itemScriptableObject.item_type);
                    item.PickItem();
                    UpdateInventoryUI();
                }
            }
        }
    }

    public void SlotClicked()
    {
        selectedItem = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < itemSprites.Count; i++)
        {
            if (i < inventoryList.Count)
            {
                itemSprites[i].sprite = itemSetActive[inventoryList[i]].GetComponent<ItemPickable>().itemScriptableObject.item_sprite;
                itemSprites[i].enabled = true;
            }
            else
            {
                itemSprites[i].sprite = emptySlotSprite;
                itemSprites[i].enabled = false;
            }
        }

        if (inventoryList.Count > 0)
        {
            int a = 0;
            foreach (Image image in itemSelectedIndicators)
            {
                if (a == selectedItem)
                {
                    image.gameObject.SetActive(true);
                }
                else
                {
                    image.gameObject.SetActive(false);
                }
                a++;
            }
        }
    }
}

public interface IPickable
{
    void PickItem();
}