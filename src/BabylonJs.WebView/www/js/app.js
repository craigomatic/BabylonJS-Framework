/// <reference path="babylon.2.0.debug.js" />
app = {};

app._engine = null;
app._scene = null;
app._canvas = null;

app._transientContents = [];

app.initScene = function (canvasId) {
    var canvas = document.getElementById(canvasId);
    
    var engine = new BABYLON.Engine(canvas, true);
    var scene = new BABYLON.Scene(engine);

    var dirLight = new BABYLON.DirectionalLight('dirLight', new BABYLON.Vector3(0,1,0), scene);
    dirLight.diffuse = new BABYLON.Color3(0.1, 0.2, 0.3);

    var arcRotateCamera = new BABYLON.ArcRotateCamera("arcCamera", 1, 0.8, 10, BABYLON.Vector3.Zero(), scene);
    arcRotateCamera.target = new BABYLON.Vector3(0, 10, 0);

    scene.activeCamera = arcRotateCamera;
    scene.activeCamera.attachControl(canvas, true);

    var debugLayer = new BABYLON.DebugLayer(scene);
    debugLayer.show(true);

    this._canvas = canvas;
    this._engine = engine;
    this._scene = scene;

    engine.runRenderLoop(function () {
        arcRotateCamera.alpha += 0.001;

        scene.render();
    });
}

app.loadBabylonModel = function (json) {

    var dataUri = "data:" + json;
    var scene = this._scene;
    var canvas = this._canvas;
    var transientContents = this._transientContents;
    
    BABYLON.SceneLoader.ImportMesh("", "/", dataUri, scene, function (meshArray) {
        meshArray[0].position = new BABYLON.Vector3(0, 0, 0);
        meshArray[0].rotation = new BABYLON.Vector3(0, 0, 0);
        meshArray[0].scaling = new BABYLON.Vector3(1, 1, 1);
        
        scene.activeCamera.setPosition(new BABYLON.Vector3(0, meshArray[0].getBoundingInfo().boundingBox.center.y, meshArray[0].getBoundingInfo().boundingSphere.radius * 4));
        scene.activeCamera.target = new BABYLON.Vector3(0, meshArray[0].getBoundingInfo().boundingBox.center.y, 0);

        //put standard material onto the mesh
        var material = new BABYLON.StandardMaterial("", scene);
        material.emissiveColor = new BABYLON.Color3(105 / 255, 113 / 255, 121 / 255);
        material.specularColor = new BABYLON.Color3(1.0, 0.2, 0.7);
        material.backFaceCulling = false;
        meshArray[0].material = material;
        
        framework.scriptNotify(JSON.stringify({ type: 'log', payload: 'mesh imported, array length was ' + meshArray.length }));

        transientContents.push(meshArray[0]);
    });
}

app.clearModelFromScene = function () {
    this._transientContents.map(function (item, idx) {
        item.dispose();
    });

    this._transientContents = [];
}