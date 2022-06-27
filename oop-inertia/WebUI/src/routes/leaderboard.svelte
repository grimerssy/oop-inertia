<script>
    import Leaderboard from "../lib/leaderboard/Leaderboard.svelte"
</script>

<div class="col">
    <h1>Top players</h1>
    <Leaderboard />
</div>

<style>
    h1 {
        margin: 1.5rem;
    }

    .col {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }
</style>