extends KinematicBody2D

export(Resource) var enemy_props = preload("res://resources/DefaultPlayer.tres")

onready var collision_shape_2d = $CollisionShape2D
var player

var velocity = Vector2.ZERO
var screen_size: Vector2

#calculated enemy parameters
var GRAVITY: float
var JUMP_VELOCITY: float
var MAX_VELOCITY_X: float

func _ready():
	screen_size = get_viewport_rect().size
	calculate_gravity_and_velocity()
	player = get_parent().get_node("MC")

func _physics_process(delta):
	apply_gravity(delta)
	velocity = move_and_slide(velocity, Vector2.UP)
	position.x = clamp(position.x, 0, screen_size.x)
	position.y = clamp(position.y, 0, screen_size.y)
	

func apply_gravity(delta):
	velocity.y += GRAVITY * delta


func calculate_gravity_and_velocity():
	GRAVITY = 8 * enemy_props.JUMP_HEIGHT / pow(enemy_props.JUMP_TIME, 2)
	JUMP_VELOCITY = 4 * enemy_props.JUMP_HEIGHT / enemy_props.JUMP_TIME
	MAX_VELOCITY_X = enemy_props.JUMP_RANGE / enemy_props.JUMP_TIME

