using System.Linq;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Nita.game;
using Nita.managers.game;
using Nita.managers.save;

namespace Nita.menu
{
  public class Menu : GameState
  {
    [Export] public float ScrollSpeed;
    [Node("CanvasLayer/UI/SavePanel")] readonly Panel _savePanel = null;

    [Node("CanvasLayer/UI/SavePanel/SavesList")]
    readonly ItemList _savesList = null;

    [Node("CanvasLayer/UI")] readonly Control _ui = null;

    public Menu() : base(nameof(Menu))
    {
    }

    public override void _Ready()
    {
      base._Ready();
      this.Wire();
      GetNode<AnimationPlayer>("AnimationPlayer").Play("spin");

      UpdateSaveFileList();
      SaveManager.CurrentSaveFile = null;
    }

    public override void _Process(float delta)
    {
      var camera = GetNode<Camera2D>("Camera2D");
      Vector2 pos = camera.GlobalPosition;
      pos.x += delta * ScrollSpeed;
      camera.GlobalPosition = pos;
    }

    public void UpdateSaveFileList()
    {
      _savesList.Clear();

      var saveFilePaths = SaveManager.SaveSlots;

      foreach (string path in saveFilePaths)
      {
        string name = path.GetFile().Replace(SaveManager.SaveFileType, "");
        // GD.Print($"Loaded {name} to menu");
        // Adds a space for formatting
        _savesList.AddItem(" " + name);
      }

      if (_savesList.GetItemCount() is 0) return;
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


      if (SaveManager.OpenSave(SaveManager.FormatAbsPath(saveName)))
      {
        GameplayScene.LoadFromSave();
        return;
      }

      _ui.GetNode<Popup>("CorruptedFilePopup").Popup_();
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
      int index = _savesList.GetSelectedItems().First();
      string saveName = _savesList.GetItemText(index).Substring(1);
      string path = SaveManager.FormatAbsPath(saveName);

      var dir = new Directory();
      dir.Remove(path);
      UpdateSaveFileList();
    }
  }
}