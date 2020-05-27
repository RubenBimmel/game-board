let networkElements = [];
let dotnet = null;
let canvas = null;
let canvasWrapper = null;
let selection = [];


/*
 *   Initialize
 */

function initialize(reference) {
    dotnet = reference;
    canvasWrapper = document.getElementById('canvas-wrapper');
    canvas = new fabric.Canvas('canvas', {
        backgroundColor: 'rgb(245,245,245)'
    });

    resize();
    window.addEventListener('resize', resize);
    canvas.on('mouse:down', onMouseDown);
    canvas.on('mouse:up', onMouseUp);
    canvas.on('object:moving', onObjectMove);
    canvas.on('object:rotating', onObjectMove);

    /*window.canvas.on('mouse:over', onMouseOver);
      window.canvas.on('mouse:out', onMouseOut);
      window.canvas.on('object:moving', onObjectMove);
    
      loadImages();*/
};

/*
 *   Fabric events
 */

function onMouseDown(options) {
    updateSelection();
    
    if (options.target) {
        return;
    }

    dotnet.invokeMethodAsync("OnAdd", {
        Top: options.pointer.y,
        Left: options.pointer.x
    });
}

function onMouseUp() {
    updateSelection()
}

function onObjectMove(options) {
    if (!isNaN(options.target.id)) {
        dotnet.invokeMethodAsync("OnMove",{
            Id: options.target.id,
            Position: {
                Top: options.target.top,
                Left: options.target.left
            },
            Angle: options.target.angle
        });
    }
    else {
        const c = new fabric.Point(0, 0);
        options.target.forEachObject(target => {
            let mGroup = options.target.calcTransformMatrix(true);
            let mObject = target.calcTransformMatrix(true);
            let mTotal = fabric.util.multiplyTransformMatrices(mGroup, mObject);
            let p = fabric.util.transformPoint(c, mTotal);
            
            dotnet.invokeMethodAsync("OnMove", {
                Id: target.id,
                Position: {
                    Top: p.y,
                    Left: p.x
                },
                Angle: options.target.angle + target.angle
            });
        })
    }
}

/*
 *   Blazor events
 */

function dispose() {
    window.removeEventListener('resize', resize);
}

function addObject(element) {
    var rect = new fabric.Rect({
        id: element.id,
        left: element.position.left,
        top: element.position.top,
        fill: 'grey',
        width: 20,
        height: 20
    });

    rect.setControlsVisibility({
        mt: false,
        mb: false,
        ml: false,
        mr: false,
        bl: false,
        br: false,
        tl: false,
        tr: false,
        mtr: true
    });

    networkElements[element.id] = rect;
    canvas.add(rect);
}

function selectObject(element, owner) {
    let selectable = element.owner ? owner : true;
    networkElements[element.id].set('stroke', element.owner?.color);
    networkElements[element.id].set('strokeWidth', element.owner ? 5 : 0);
    networkElements[element.id].set('selectable', selectable);
    networkElements[element.id].set('hoverCursor', selectable ? "move" : "not-allowed");
    canvas.renderAll();
}

function moveObject(element) {
    networkElements[element.id].top = element.position.top;
    networkElements[element.id].left = element.position.left;
    networkElements[element.id].angle = element.angle;
    networkElements[element.id].setCoords();
    canvas.renderAll();
}

/*
 *   Helper functions
 */

function resize() {
    canvas.setWidth(canvasWrapper.clientWidth);
    canvas.setHeight(canvasWrapper.clientHeight);
    canvas.calcOffset();
}

function updateSelection() {
    let newSelection = canvas.getActiveObjects().map(o => o.id);
    
    if (selection.length === newSelection.length && selection[0] === newSelection[0]) {
        return;
    }

    dotnet.invokeMethodAsync("OnSelect", newSelection.filter(o => !selection.includes(o)));
    dotnet.invokeMethodAsync("OnDeselect", selection.filter(o => !newSelection.includes(o)));
    selection = newSelection;
}

/*function onMouseOver(options) {
  if (!options.target || options.target === this.selected) return;
  this.highlight(options.target, 1);
}

function onMouseOut(options) {
  if (!options.target || options.target === this.selected) return;
  this.highlight(options.target, 0);
}

function onObjectMove(options) {
  if (options.target !== this.selected) return;
  
  this.overlap(null);
  options.target.setCoords();
  this.canvas.forEachObject(this.checkForOverlap);
  this.canvas.renderAll();
}

function checkForOverlap(target) {
  if (target === this.selected) return;
  if (!this.selected.intersectsWithObject(target)) return;
  if (this.overlapped && distance(this.overlapped, this.selected) < distance(target, this.selected)) return;
  this.overlap(target);
}

function overlap(target) {
  if (this.overlapped) {
    this.highlight(this.overlapped, 0);
  }

  if (target) {
    this.highlight(target, 1);
  }

  this.overlapped = target;
}

function select(target) {
  if(this.selected === target && Date.now() - this.lastClick < 200) {
    target.data.onDoubleClick();
  }

  if (this.selected) {
    this.highlight(this.selected, 0);
    this.overlap(null);
  }

  if (target) {
    this.highlight(target, 3);
    this.lastClick = Date.now();
  }

  this.selected = target;
}

function highlight(target, width, render) {
  target.set('stroke', 'red');
  target.set('strokeWidth', width);
  if (render) this.canvas.renderAll();
}

function addCard(card, x, y) {
  var imgInstance = new fabric.Image(this.images.deckOfCards['deck'], {
    left: x,
    top: y
  });

  imgInstance.setControlsVisibility({
     mt: false, 
     mb: false, 
     ml: false, 
     mr: false, 
     bl: false,
     br: false, 
     tl: false, 
     tr: false,
     mtr: true, 
  });

  imgInstance.data = {
    card: card,
    faceUp: false,
    onDoubleClick: () => {
      imgInstance.data.faceUp = !imgInstance.data.faceUp;
      imgInstance.setElement(imgInstance.data.faceUp ? this.images.deckOfCards[imgInstance.data.card] : this.images.deckOfCards['deck']);
    }
  }
  
  this.canvas.add(imgInstance);
}

async function loadImages() {
  this.images.deckOfCards = await DeckOfCards();
}*/
