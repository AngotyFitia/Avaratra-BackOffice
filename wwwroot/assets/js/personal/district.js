document.getElementById('btnImportCsv').addEventListener('click', function (e) {
    e.preventDefault(); 
    document.getElementById('csvFileInput').click();
});

document.getElementById('csvFileInput').addEventListener('change', function () {
    this.form.submit();
});

document.addEventListener('DOMContentLoaded', function () {
    var editModal = document.getElementById('editModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            editModal.querySelector('#districtId').value = button.getAttribute('data-id');
            editModal.querySelector('#districtName').value = button.getAttribute('data-name');
            editModal.querySelector('#districtPop').value = button.getAttribute('data-pop');

            // Sélectionner la région actuelle
            var regionSelect = editModal.querySelector('#regionList');
            var currentRegionId = button.getAttribute('data-regionid');
            if (regionSelect && currentRegionId) {
                regionSelect.value = currentRegionId;
            }
        });
    }

    var deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            deleteModal.querySelector('#districtId').value = button.getAttribute('data-id');
            deleteModal.querySelector('#districtName').textContent = button.getAttribute('data-name');
        });
    }

    var validateModal = document.getElementById('validateModal');
    if (validateModal) {
        validateModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            console.log(button.getAttribute('data-id'));
            validateModal.querySelector('#districtId').value = button.getAttribute('data-id');
            validateModal.querySelector('#districtName').textContent = button.getAttribute('data-name');
        });
    }

    var finalModal = document.getElementById('finalModal');
    if (finalModal) {
        finalModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            finalModal.querySelector('#districtId').value = button.getAttribute('data-id');
            finalModal.querySelector('#districtName').textContent = button.getAttribute('data-name');
        });
    }

    var modal = document.getElementById('viewModal');
        var map, vectorLayer;
    
        modal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            var wkt = button.getAttribute('data-geometry'); // format POLYGON ((47.5 -18.9, 47.6 -18.9, ...))
    
            // Parse WKT avec OpenLayers
            var format = new ol.format.WKT();
            var feature = format.readFeature(wkt, {
                dataProjection: 'EPSG:4326',
                featureProjection: 'EPSG:3857'
            });
    
            // Création de la source et du layer
            var vectorSource = new ol.source.Vector({
                features: [feature]
            });
    
            vectorLayer = new ol.layer.Vector({
                source: vectorSource,
                style: new ol.style.Style({
                    stroke: new ol.style.Stroke({
                        color: 'red',
                        width: 2
                    }),
                    fill: new ol.style.Fill({
                        color: 'rgba(255,0,0,0.3)'
                    })
                })
            });
    
            // Initialisation de la carte si elle n'est  pas encore créée
            if (!map) {
                map = new ol.Map({
                    target: 'map',
                    layers: [
                        new ol.layer.Tile({
                            source: new ol.source.OSM()
                        }),
                        vectorLayer
                    ],
                    // view: new ol.View({
                    //     center: ol.proj.fromLonLat([47.5, -18.9]),
                    //     zoom: 6
                    // })
                });
            } else {
                map.setLayers([
                    new ol.layer.Tile({ source: new ol.source.OSM() }),
                    vectorLayer
                ]);
            }
    
            // Ajustement de la vue sur le polygone
            var extent = feature.getGeometry().getExtent();
            map.getView().fit(extent, { size: map.getSize(), padding: [20,20,20,20], maxZoom:10 });
        });  
});


