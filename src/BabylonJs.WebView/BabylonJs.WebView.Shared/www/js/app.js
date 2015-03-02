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
    
    var babylonCamera = new BABYLON.FreeCamera('cam', new BABYLON.Vector3(0, 0, -10), scene);
    babylonCamera.attachControl(canvas, true);

    scene.setActiveCameraByID(babylonCamera.id);

    this._transientContents.push(babylonCamera);

    //var dirLight = new BABYLON.DirectionalLight("dirLight", new BABYLON.Vector3(0, 1, 0), scene);
    //dirLight.range = 100;
    //dirLight.specular = new BABYLON.Color3(237 / 255, 95 / 255, 95 / 255);
    //dirLight.diffuse = new BABYLON.Color3(173 / 255, 173 / 255, 171 / 255);

    var debugLayer = new BABYLON.DebugLayer(scene);
    debugLayer.show(true);

    this._canvas = canvas;
    this._engine = engine;
    this._scene = scene;

    engine.runRenderLoop(function () {
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
        
        //put standard material onto the mesh
        var material = new BABYLON.StandardMaterial("", scene);
        material.emissiveColor = new BABYLON.Color3(105 / 255, 113 / 255, 121 / 255);
        material.specularColor = new BABYLON.Color3(1.0, 0.2, 0.7);
        material.backFaceCulling = false;
        meshArray[0].material = material;
        
        framework.scriptNotify(JSON.stringify({ type: 'log', payload: 'mesh imported, array length was ' + meshArray.length }));

        //switch to an arc rotate camera focused on this object
        var arcRotateCamera = new BABYLON.ArcRotateCamera("arcCamera", 0, 0, 0, BABYLON.Vector3.Zero(), scene);
        arcRotateCamera.setPosition(new BABYLON.Vector3(0, meshArray[0].getBoundingInfo().boundingBox.center.y, meshArray[0].getBoundingInfo().boundingSphere.radius * -1));

        scene.activeCamera = arcRotateCamera;
        scene.activeCamera.attachControl(canvas);

        transientContents.push(arcRotateCamera);
        transientContents.push(meshArray[0]);
    });
}

app.clearModelFromScene = function () {
    this._transientContents.map(function (item, idx) {
        item.dispose();
    });

    this._transientContents = [];
}