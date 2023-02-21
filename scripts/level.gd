extends Node2D

func _ready():
	VisualServer.set_default_clear_color(Color.cadetblue)

func travel_data_feed(position_of_nav_tile, path_of_node):
	GlobalData.travel_nodes[position_of_nav_tile] = path_of_node
