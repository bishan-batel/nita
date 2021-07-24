using Godot;
using Parry2.game;
using Parry2.managers.game;
using Parry2.managers.save;

namespace Parry2.menu
{
  public class Menu : GameState
  {
    Panel _savePanel;
    ItemList _savesList;
    Control _ui;

    public Menu() : base(nameof(Menu))
    {
    }

    public override void _Ready()
    {
      base._Ready();
      GetNode<AnimationPlayer>("AnimationPlayer").Play("spin");

      _ui = GetNode<Control>("CanvasLayer/UI");
      _savePanel = _ui.GetNode<Panel>("SavePanel");
      _savesList = _savePanel.GetNode<ItemList>("SavesList");

#if DEBUG
      GD.Print("Updating save file list");
#endif
      UpdateSaveFileList();
      SaveManager.CurrentSaveFile = null;
    }

    public void UpdateSaveFileList()
    {
      _savesList.Clear();

      var saveFilePaths = SaveManager.SaveSlots;

      foreach (string path in saveFilePaths)
      {
        string name = SaveManager.GetSaveFileName(path);
        // GD.Print($"Loaded {name} to menu");
        // Adds a space for formatting
        _savesList.AddItem(" " + name);
      }

      if (_savesList.GetItemCount() == 0) return;
      _savePanel.GetNode<Button>("PlayButton").Disabled = true;
      _savePanel.GetNode<Button>("DeleteButton").Disabled = true;
    }

    public void _on_SavesList_item_selected(int index)
    {
      _savePanel.GetNode<Button>("PlayButton").Disabled = false;
      _savePanel.GetNode<Button>("DeleteButton").Disabled = false;
    }

    public void _on_PlayButton_pressed()
    {
      int index = _savesList.GetSelectedItems()[0];

      // Removes the space added before
      string saveName = _savesList.GetItemText(index).Substring(1);

      bool success = SaveManager.OpenSave(SaveManager.FormatAbsPath(saveName));

      if (!success)
      {
        _ui.GetNode<Popup>("CorruptedFilePopup").Popup_();
        return;
      }

      GameplayScene.LoadFromSave(GetTree(), SaveManager.CurrentSaveFile);
    }

    public void _on_CreateButton_pressed()
    {
      bool success = SaveManager.CreateNewSave();
      if (!success)
        _ui.GetNode<Popup>("FailedCreatePopup")?.Popup_();

      UpdateSaveFileList();
    }

    public void _on_DeleteButton_pressed()
    {
      int index = _savesList.GetSelectedItems()[0];
      string saveName = _savesList.GetItemText(index).Substring(1);
      string path = SaveManager.SaveDirPath + saveName + SaveManager.SaveFileType;
      var dir = new Directory();
      dir.Remove(path);
      UpdateSaveFileList();
    }
  }
}