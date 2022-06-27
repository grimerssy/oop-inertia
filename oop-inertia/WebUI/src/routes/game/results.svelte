<script>
    import {bestPlayer} from "./store.js"
    import Leaderboard from "../../lib/leaderboard/Leaderboard.svelte";

</script>

<div class="row">
    <h1 class="w-50">Top players</h1>
    <h1 class="w-50">Winner</h1>
</div>
<div class="row">
    <Leaderboard />
    <div class="col w-50">
        <h2 style="color: {bestPlayer.color}">Name: {bestPlayer.name}</h2>
        <h2 style="color: {bestPlayer.color}">Points: {Math.floor(bestPlayer.score)}</h2>
    </div>
</div>

<style>
    h1 {
        margin: 1.5rem;
    }

    h2 {
        opacity: .85;
    }

    .col {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }

    .row {
        display: flex;
        align-items: center;
        justify-content: space-evenly;
    }

    .w-50 {
        width: 50%;
    }
</style>