import { AppService } from './app.service';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { Card } from './_core/models/card.model';
import { NgxMasonryOptions } from 'ngx-masonry';
import { ButtonHelper } from './_core/helpers/button.helper';
import * as THREE from 'three';

const MOUSE_WHEEL_EVENT = 'wheel';
const TOUCH_MOVE = 'touchmove';
const TOUCH_END = 'touchend';
const MOUSE_DOWN = 'mousedown';
const MOUSE_UP = 'mouseup';
const MOUSE_MOVE = 'mousemove';
const IMAGE_SIZE = 512;
const SCROLL_PER_IMAGE = 500;
const renderer = new THREE.WebGLRenderer({ antialias: false });
const VERTEX_SHADER = `
precision mediump float;
precision mediump int;
attribute vec4 color;
varying vec3 vPosition;
varying vec4 vColor;
varying vec2 vUv;
void main()	{
  vUv = uv;
  vPosition = position;
  vColor = color;
  gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1);
}`;
const FRAGMENT_SHADER = `
precision mediump float;
precision mediump int;
uniform float time;
uniform float blend;
varying vec3 vPosition;
varying vec4 vColor;

uniform sampler2D tex1;
uniform sampler2D tex2;
varying vec2 vUv;

float length = 10.;

mat2 scale(vec2 _scale){
  return mat2(_scale.x,0.0,
              0.0,_scale.y);
}

mat3 k = mat3(
         -0.3, 0., 1.,
         -0.4, 0., 1.,
        2., 0., 1.
        );

float displaceAmount = 0.3;

void main()	{
  // invert blend;
  float blend2 = 1.-blend;
  vec4 image1 = texture2D(tex1, vUv);
  vec4 image2 = texture2D(tex2, vUv);

  float t1 = ((image2.r*displaceAmount)*blend)*2.;
  float t2 = ((image1.r*displaceAmount)*blend2)*2.;

  vec4 imageA = texture2D(tex2, vec2(vUv.x, vUv.y-t1))*blend2;
  vec4 imageB = texture2D(tex1, vec2(vUv.x, vUv.y+t2))*blend;

  gl_FragColor = imageA.bbra * blend + imageA * blend2 +
  imageB.bbra * blend2 + imageB * blend;

  //gl_FragColor = image3;

}`;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  card$: Observable<Array<Card>>;
  isLoading: boolean = null;
  loaderSub: Subscription;
  hideSearchbar = false;

  title = 'Spiritmonger';

  searchForm: FormGroup;
  namePart: FormControl;

  masonryOptions: NgxMasonryOptions = {
    transitionDuration: '0.2s',
    gutter: 0,
    resize: true,
    fitWidth: true,
    horizontalOrder: true,
    percentPosition: true,
    initLayout: true
  };

  // animation
  canvas: HTMLCanvasElement = null;
  ctx: CanvasRenderingContext2D;
  mouseWheel = new ScrollPos();

  constructor(private builder: FormBuilder, private cardService: AppService) {}

  ngOnInit() {
    this.createForm();
    this.card$ = this.cardService.cards;
    this.loaderSub = this.cardService.isLoading.subscribe(value => {
      this.isLoading = value;
      // if (this.isLoading === false) {
      //   //ButtonHelper.toggleButtonById('moreBtn', 'More');
      //   //ButtonHelper.toggleButtonById('searchBtn', 'GO');
      // } else {
      //   //ButtonHelper.toggleButtonById('moreBtn');
      //   //ButtonHelper.toggleButtonById('searchBtn');
      // }
    });

    this.card$.subscribe(cards => {
      if (cards.length <= 0) return;
      if (!this.canvas) {
        this.canvas = document.createElement('canvas');
        // document.getElementById("canvas").appendChild(this.canvas);
      } else {
        this.canvas.remove();
        this.ctx.canvas.remove();
        this.canvas = document.createElement('canvas');
      }
      this.hideSearchbar = true;
      this.canvas.width = IMAGE_SIZE;
      this.canvas.height = IMAGE_SIZE;
      this.canvas.style.marginTop = '-32px';
      this.canvas.style.display = 'block';
      this.ctx = this.canvas.getContext('2d');

      this.loadImages(cards).then(images => {
        this.init(images);
      });

      document.body.appendChild(renderer.domElement);
    });
  }

  ngOnDestroy() {
    this.loaderSub.unsubscribe();
  }

  showMore = (): void => {
    this.cardService.nextPage();
  };

  createForm() {
    this.namePart = new FormControl('', [Validators.required, Validators.minLength(4)]);
    this.searchForm = this.builder.group({
      namePart: this.namePart
    });
  }

  updateName = () => {
    this.namePart.markAsTouched();
    if (!this.searchForm.valid || this.isLoading) {
      return;
    }
    this.cardService.startSearch(this.namePart.value);
    this.title = this.namePart.value;
  };

  closeImages = () => {
    document.getElementsByTagName('canvas')[0].style.display = 'none';
    this.hideSearchbar = false;
  };

  // three animation
  resizeImage = (image, size = IMAGE_SIZE) => {
    let newImage = image;
    let { width, height } = image;
    let newWidth = size / width;
    let newHeight = size / height;

    this.ctx.drawImage(image, 0, 0, width, height, 0, 0, size, size);

    return this.ctx.getImageData(0, 0, size, size);
  };

  makeThreeTexture = image => {
    let tex = new THREE.Texture(image);
    tex.needsUpdate = true;
    return tex;
  };

  loadImages = (cards: Card[]) => {
    let promises = [];
    for (var i = 0; i < cards.length; i++) {
      //for (var i = 0; i < this.files.length; i++) {
      promises.push(
        new Promise((resolve, reject) => {
          if (cards[i].imageUrl.indexOf('gatherer.wizards.com') == -1) {
            let img = document.createElement('img');
            img.crossOrigin = 'anonymous';
            img.src = `${cards[i].imageUrl}`;
            // img.src = `${this.files[i]}`;

            img.onload = image => {
              return resolve(image.target);
            };
          }
        })
          .then(this.resizeImage)
          .then(this.makeThreeTexture)
      );
    }
    return Promise.all(promises);
  };

  init = textures => {
    let mouseWheel = this.mouseWheel;
    let scene = new THREE.Scene();
    let camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 2000);
    camera.position.set(0, 0, 10);

    scene.add(camera);

    let geometry = new THREE.PlaneGeometry(4.75, 7, 4, 4);

    let material = new THREE.ShaderMaterial({
      uniforms: {
        time: { value: 1.0 },
        blend: { value: 0.0 },
        tex1: { type: 't', value: textures[1] },
        tex2: { type: 't', value: textures[0] }
      },
      vertexShader: VERTEX_SHADER,
      fragmentShader: FRAGMENT_SHADER
    });

    let mesh = new THREE.Mesh(geometry, material);

    scene.add(mesh);

    var tex1 = textures[1];
    var tex2 = textures[0];

    function updateTexture(pos) {
      if (tex2 != textures[Math.floor(pos / SCROLL_PER_IMAGE)]) {
        tex2 = textures[Math.floor(pos / SCROLL_PER_IMAGE)];
        material.uniforms.tex2.value = tex2;
      }
      if (tex1 != textures[Math.floor(pos / SCROLL_PER_IMAGE) + 1]) {
        tex1 = textures[Math.floor(pos / SCROLL_PER_IMAGE) + 1];
        material.uniforms.tex1.value = tex1;
      }
    }

    function draw() {
      requestAnimationFrame(draw);
      mouseWheel.update();
      let scrollTarget = Math.floor((mouseWheel.scrollPos + SCROLL_PER_IMAGE * 0.5) / SCROLL_PER_IMAGE) * SCROLL_PER_IMAGE;
      mouseWheel.snap(scrollTarget);

      let { scrollPos, velocity } = mouseWheel;

      if (scrollPos < 0) {
        scrollPos = 0;
      }
      if (scrollPos > SCROLL_PER_IMAGE * textures.length - 1) {
        scrollPos = SCROLL_PER_IMAGE * textures.length - 1;
      }

      if (scrollPos > 0 && scrollPos < SCROLL_PER_IMAGE * textures.length - 1) {
        updateTexture(scrollPos);
        material.uniforms.blend.value = (scrollPos % SCROLL_PER_IMAGE) / SCROLL_PER_IMAGE;
      }

      mouseWheel.scrollPos = scrollPos;

      material.uniforms.time.value += 0.1;

      renderer.render(scene, camera);
    }

    function resize() {
      camera.aspect = window.innerWidth / window.innerHeight;
      camera.updateProjectionMatrix();
      renderer.setPixelRatio(window.devicePixelRatio);
      renderer.setSize(window.innerWidth, window.innerHeight);
    }

    window.addEventListener('resize', resize);

    resize();
    draw();
  };
}

export class ScrollPos {
  constructor() {
    this.acceleration = 0;
    this.maxAcceleration = 5;
    this.maxSpeed = 20;
    this.velocity = 0;
    this.dampen = 0.97;
    this.speed = 8;
    this.touchSpeed = 8;
    this.scrollPos = 0;
    this.velocityThreshold = 1;
    this.snapToTarget = false;
    this.mouseDown = false;
    this.lastDelta = 0;

    window.addEventListener(MOUSE_WHEEL_EVENT, event => {
      event.preventDefault();
      this.accelerate(Math.sign(event.deltaY) * this.speed);
    });

    window.addEventListener(TOUCH_MOVE, event => {
      //event.preventDefault();
      let delta = this.lastDelta - event.targetTouches[0].clientY;
      this.accelerate(Math.sign(delta) * this.touchSpeed);
      this.lastDelta = event.targetTouches[0].clientY;
    });

    window.addEventListener(TOUCH_END, event => {
      this.lastDelta = 0;
    });

    window.addEventListener(MOUSE_DOWN, event => {
      this.mouseDown = true;
    });

    window.addEventListener(MOUSE_MOVE, event => {
      if (this.mouseDown) {
        let delta = this.lastDelta - event.clientY;
        this.accelerate(Math.sign(delta) * this.touchSpeed * 0.4);
        this.lastDelta = event.clientY;
      }
    });

    window.addEventListener(MOUSE_UP, event => {
      this.lastDelta = 0;
      this.mouseDown = false;
    });
  }
  accelerate(amount) {
    if (this.acceleration < this.maxAcceleration) {
      this.acceleration += amount;
    }
  }
  update() {
    this.velocity += this.acceleration;
    if (Math.abs(this.velocity) > this.velocityThreshold) {
      this.velocity *= this.dampen;
      this.scrollPos += this.velocity;
    } else {
      this.velocity = 0;
    }
    if (Math.abs(this.velocity) > this.maxSpeed) {
      this.velocity = Math.sign(this.velocity) * this.maxSpeed;
    }
    this.acceleration = 0;
  }
  snap(snapTarget, dampenThreshold = 100, velocityThresholdOffset = 1.5) {
    if (Math.abs(snapTarget - this.scrollPos) < dampenThreshold) {
      this.velocity *= this.dampen;
    }
    if (Math.abs(this.velocity) < this.velocityThreshold + velocityThresholdOffset) {
      this.scrollPos += (snapTarget - this.scrollPos) * 0.1;
    }
  }
  project(steps = 1) {
    if (steps === 1) return this.scrollPos + this.velocity * this.dampen;
    var scrollPos = this.scrollPos;
    var velocity = this.velocity;

    for (var i = 0; i < steps; i++) {
      velocity *= this.dampen;
      scrollPos += velocity;
    }
    return scrollPos;
  }
  acceleration = 0;
  maxAcceleration = 5;
  maxSpeed = 20;
  velocity = 0;
  dampen = 0.97;
  speed = 8;
  touchSpeed = 8;
  scrollPos = 0;
  velocityThreshold = 1;
  snapToTarget = false;
  mouseDown = false;
  lastDelta = 0;
  scrollPerImage = 500;
}

const KEYBOARD_ACCELERATION = 25;

window.addEventListener('keydown', e => {
  switch (e.keyCode) {
    case 33:
    case 38:
      // UP
      this.mouseWheel.acceleration -= KEYBOARD_ACCELERATION;
      this.mouseWheel.update();
      break;
    case 34:
    case 40:
      // DOWN
      this.mouseWheel.acceleration += KEYBOARD_ACCELERATION;
      this.mouseWheel.update();
      break;
  }
});
