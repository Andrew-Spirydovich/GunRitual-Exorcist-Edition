shader_type canvas_item;

// Базовый жёлтый, но бледнее и с полупрозрачностью
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 0.0, 0.5);
// Цвет для углов — тоже приглушённый
uniform vec4 corner_tint   : source_color = vec4(1.0, 0.6, 0.0, 0.5);

uniform int outline_size = 1;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);

    if (tex.a == 0.0) {
        bool draw_outline = false;
        bool is_corner = false;

        for (int x = -outline_size; x <= outline_size && !draw_outline; x++) {
            for (int y = -outline_size; y <= outline_size && !draw_outline; y++) {
                vec2 offset = vec2(float(x), float(y)) * TEXTURE_PIXEL_SIZE;
                if (texture(TEXTURE, UV + offset).a > 0.0) {
                    draw_outline = true;
                    if (x != 0 && y != 0) {
                        is_corner = true;
                    }
                }
            }
        }

        if (draw_outline) {
            COLOR = is_corner ? corner_tint : outline_color;
        } else {
            discard;
        }
    } else {
        COLOR = tex;
    }
}
