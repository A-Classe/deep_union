# frozen_string_literal: true

require "ruby2d"

def random_in_circle(cx, cy, r)
  theta = 2.0 * Math::PI * rand
  radius = r * Math.sqrt(rand)
  [cx + radius * Math.cos(theta), cy + radius * Math.sin(theta)]
end
class CircleDistributor
  attr_reader :points

  def initialize(cx, cy, r)
    @cx, @cy, @r = cx, cy, r
    @points = []
  end

  # 指定された数の点を追加
  def add_points(n)
    n.times do
      @points << random_in_circle(@cx, @cy, @r)
    end
  end
end
class App
  include Ruby2D::DSL

  def call
    # https://www.ruby2d.com/learn/window/#setting-attributes
    set title: "(Ruby 2D)"
    set width: 800, height: 600

    frame_count = 0

    def random_in_circle(cx, cy, r)
      theta = 2.0 * Math::PI * rand
      radius = r * Math.sqrt(rand)
      [cx + radius * Math.cos(theta), cy + radius * Math.sin(theta)]
    end

    # 使用例:
    cx, cy = 1, 1  # 円の中心
    r = 5          # 円の半径
    n = 30         # 最初の点の数

    distributor = CircleDistributor.new(cx, cy, r)
    distributor.add_points(n)


    update do
      # 描画オブジェクトたちをぶっ殺す (超重要)
      # ただしこのような使い方をフレームワーク開発者は推奨してないと思われる
      clear

      # https://www.ruby2d.com/learn/window/#getting-attributes
      w, h = get(:width), get(:height)

      # https://www.ruby2d.com/learn/text/
      objects_count = DSL.window.instance_variable_get(:@objects).count
      text = [frame_count, get(:fps), objects_count] * " "
      text_object = Text.new(text, x: 0, y: 0)


      distributor.points.each { |p|
        # print p
        Circle.new(
          x: p[0]*10 + 200, y: p[1]*10+ 200,
          radius: 2,
          sectors: 32,
          color: 'red',
          z: 10
        )
      }

      if false
        # 直接描画する場合はこのようにすればよいのかと思ったが canvas も同様にオブジェクトだった
        # また text を canvas に描画する方法は用意されていない
        canvas = Canvas.new(width: w, height: h)

        canvas.fill_rectangle(
          x: w * 0.25, y: h * 0.25,
          width: w * 0.5, height: h * 0.5,
          color: "blue")

        canvas.draw_line(
          x1: 0, y1: 0,
          x2: w - 1, y2: h - 1,
          color: "white")
      end

      frame_count += 1
    end

    # https://www.ruby2d.com/learn/input/#keyboard
    on :key_down do |event|
      if event.key == "escape" || event.key == "q"
        close
      end
      if event.key == "w"
        # 点をさらに追加
        additional_points = 10
        distributor.add_points(additional_points)
      end
    end
    show
  end

  new.call
end