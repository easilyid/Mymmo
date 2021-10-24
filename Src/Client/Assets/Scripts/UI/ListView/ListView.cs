using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[System.Serializable]
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem>
{

}
public class ListView : MonoBehaviour
{
    public UnityAction<ListViewItem> OnItemSelected;
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnSelected(selected);
            }
        }
        public virtual void OnSelected(bool selected)
        {
        }

        public ListView Owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.selected)
            {
                this.Selected = true;
            }
            if (Owner != null && Owner.SelectedItem != this)
            {
                Owner.SelectedItem = this;
            }
        }
    }

    readonly List<ListViewItem> items = new List<ListViewItem>();

    private ListViewItem selectedItem = null;
    public ListViewItem SelectedItem
    {
        get => selectedItem;
        private set
        {
            if (selectedItem!=null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;
            OnItemSelected?.Invoke(value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.Owner = this;
        this.items.Add(item);
    }

    public void RemoveAll()
    {
        foreach(var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();
    }
}
