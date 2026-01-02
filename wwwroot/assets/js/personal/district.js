// Import CSV
document.addEventListener("DOMContentLoaded", function () {
    const btnImportCsv = document.getElementById("btnImportCsv");
    const csvFileInput = document.getElementById("csvFileInput");

    if (btnImportCsv && csvFileInput) {
        btnImportCsv.addEventListener("click", function (e) {
            e.preventDefault();
            csvFileInput.click();
        });

        csvFileInput.addEventListener("change", function () {
            if (this.files.length > 0) {
                this.form.submit();
            }
        });
    }

    // Modals
    // Edit Modal
    const editModal = document.getElementById("editModal");
    if (editModal) {
        editModal.addEventListener("show.bs.modal", function (event) {
            const button = event.relatedTarget;
            editModal.querySelector("#districtId").value = button.getAttribute("data-id");
            editModal.querySelector("#districtName").value = button.getAttribute("data-name");
            editModal.querySelector("#districtPop").value = button.getAttribute("data-pop");

            const regionSelect = editModal.querySelector("#regionList");
            const currentRegionId = button.getAttribute("data-regionid");
            if (regionSelect && currentRegionId) {
                regionSelect.value = currentRegionId;
            }
        });
    }

    // Delete Modal
    const deleteModal = document.getElementById("deleteModal");
    if (deleteModal) {
        deleteModal.addEventListener("show.bs.modal", function (event) {
            const button = event.relatedTarget;
            deleteModal.querySelector("#districtId").value = button.getAttribute("data-id");
            deleteModal.querySelector("#districtName").textContent = button.getAttribute("data-name");
        });
    }

    // Validate Modal
    const validateModal = document.getElementById("validateModal");
    if (validateModal) {
        validateModal.addEventListener("show.bs.modal", function (event) {
            const button = event.relatedTarget;
            validateModal.querySelector("#districtId").value = button.getAttribute("data-id");
            validateModal.querySelector("#districtName").textContent = button.getAttribute("data-name");
        });
    }

    // Final Modal
    const finalModal = document.getElementById("finalModal");
    if (finalModal) {
        finalModal.addEventListener("show.bs.modal", function (event) {
            const button = event.relatedTarget;
            finalModal.querySelector("#districtId").value = button.getAttribute("data-id");
            finalModal.querySelector("#districtName").textContent = button.getAttribute("data-name");
        });
    }

    // View Modal with OpenLayers
  const viewModal = document.getElementById("viewModal");
  let map, vectorLayer;

  if (viewModal) {
    viewModal.addEventListener("shown.bs.modal", function (event) {
      const button = event.relatedTarget;
      const id = button.getAttribute("data-id");
      const wkt = button.getAttribute("data-geometry");
      const district = button.getAttribute("data-name");
      const totalPopulationDistrict = button.getAttribute("data-pop");

      // Charger les communes depuis le back
      fetch(`/Districts?handler=Communes&id=${id}`)
        .then(resp => resp.json())
        .then(communes => {
          // Transformer chaque commune en point OpenLayers
          const pointFeatures = communes.map(c => new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([c.longitude, c.latitude])),
            name: c.intitule,
            nombrePopulation: c.nombrePopulation,
            district: district,
            totalPopulationDistrict: totalPopulationDistrict
          }));

          const markerSource = new ol.source.Vector({ features: pointFeatures });
          const markerLayer = new ol.layer.Vector({
            source: markerSource,
            style: new ol.style.Style({
              image: new ol.style.Circle({
                radius: 6,
                fill: new ol.style.Fill({ color: "blue" }),
                stroke: new ol.style.Stroke({ color: "white", width: 2 })
              })
            })
          });

          // Charger le WKT du district
          const format = new ol.format.WKT();
          const feature = format.readFeature(wkt, {
            dataProjection: "EPSG:4326",
            featureProjection: "EPSG:3857"
          });

          const vectorSource = new ol.source.Vector({ features: [feature] });
          vectorLayer = new ol.layer.Vector({
            source: vectorSource,
            style: new ol.style.Style({
              stroke: new ol.style.Stroke({ color: "red", width: 2 }),
              fill: new ol.style.Fill({ color: "rgba(255,0,0,0.3)" })
            })
          });

          

          // Initialiser ou mettre à jour la carte
          if (!map) {
            map = new ol.Map({
              target: "map",
              layers: [
                new ol.layer.Tile({ source: new ol.source.OSM() }),
                vectorLayer,
                markerLayer
              ],
              view: new ol.View({
                center: [0, 0],
                zoom: 2
              })
            });
          } else {
            map.setLayers([
              new ol.layer.Tile({ source: new ol.source.OSM() }),
              vectorLayer,
              markerLayer
            ]);
          }
          const tooltipEl = document.getElementById("tooltip"); 
          const tooltipOverlay = new ol.Overlay({ element: tooltipEl, offset: [0, -10], 
            // petit décalage au-dessus du point 
            positioning: "bottom-center" }); map.addOverlay(tooltipOverlay);
          // Ajuster la vue
          const extent = feature.getGeometry().getExtent();
          map.getView().fit(extent, { padding: [20,20,20,20], maxZoom: 10 });
          map.on("pointermove", function (evt) {
            const hit = map.forEachFeatureAtPixel(
              evt.pixel,
              (f, layer) => (layer === markerLayer && f.getGeometry() instanceof ol.geom.Point) ? f : null,
              { hitTolerance: 8 }
            );
          
            if (hit) {
              const coord = hit.getGeometry().getCoordinates();
              tooltipOverlay.setPosition(coord);
              const pop = hit.get("nombrePopulation");
              tooltipEl.innerHTML = `Commune: ${hit.get("name")}${pop != null ? "<br>Population: " + pop : "" } <br>District: ${hit.get("district")}`;
              tooltipEl.style.display = "block";
              map.getViewport().style.cursor = "pointer";
            } else {
              tooltipEl.style.display = "none";
              map.getViewport().style.cursor = "";
            }
          });
          map.on("click", function (evt) {
            map.forEachFeatureAtPixel(evt.pixel, function (feature) {
                if (feature.get("district")) {
                    alert("District: " + feature.get("district") + "\nNombre de population: " + feature.get("totalPopulationDistrict"));
                }
            })
          });   
          
          setTimeout(() => map.updateSize(), 300);
        })
        .catch(err => {
          console.error("Erreur communes:", err);
        });
    });
  }
});


