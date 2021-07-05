extends Node2D

func _ready():
	pass # Replace with function body.
func _on_area2D_Entered(node):
    node.queueFree();
    # death code or something
